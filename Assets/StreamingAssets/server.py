import sys
import zmq
from mss import mss

REQUEST_TIMEOUT = 2500

context = zmq.Context()

print("I: Connecting to server…")
client = context.socket(zmq.REP)
client.bind("tcp://*:6789")

monitor = {"top": int(sys.argv[1]), "left": int(sys.argv[2]), "width": int(sys.argv[3]), "height": int(sys.argv[4]), "mon": int(sys.argv[5])}

poll = zmq.Poller()
poll.register(client, zmq.POLLIN)
while True:
    with mss() as sct:
        expect_reply = True
        while expect_reply:
            socks = dict(poll.poll(REQUEST_TIMEOUT))
            if socks.get(client) == zmq.POLLIN:
                reply = client.recv()
                if not reply:
                    break
                expect_reply = False
            else:
                print("W: No response from server, retrying…")
                # Socket is confused. Close and remove it.
                client.setsockopt(zmq.LINGER, 0)
                client.close()
                poll.unregister(client)
                print("I: Reconnecting and resending")
                # Create new connection
                client = context.socket(zmq.REQ)
                client.bind("tcp://*:6789")
                poll.register(client, zmq.POLLIN)

            client.send(sct.grab(monitor).raw)

context.term()
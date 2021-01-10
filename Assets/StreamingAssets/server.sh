#!/bin/sh
BASEDIR=$(dirname $0)
${BASEDIR}/venv/bin/python ${BASEDIR}/server.py ${1} ${2} ${3} ${4} ${5} ${6}

import sys


def wait_for_input():
    if input() == "$$end_wait$$":
        return
    else:
        raise Exception("Got input while waiting for end_wait")


def write(msg):
    sys.stdout.write(msg + '\n')


def read():
    return input()

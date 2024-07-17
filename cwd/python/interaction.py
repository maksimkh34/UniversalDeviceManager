import sys


def wait_for_input():
    return input() == "$$end_wait$$"


def write(msg):
    sys.stdout.write(msg + '\n')


def read():
    return input()


def start_proc():
    sys.stdout.write("$$start_proc$$")


def end_proc():
    sys.stdout.write("$$end_proc$$")


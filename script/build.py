from subprocess import run
from os import environ as env


def restore():
    print("---: Restoring Packages")
    cmd = run("dotnet restore", shell=True)
    if cmd.returncode != 0:
        print("---: Restore Failed!")
        exit(1)


def build():
    print("---: Building Projects")
    cmd = run("dotnet build", shell=True)
    if cmd.returncode != 0:
        print("---: Build Failed!")
        exit(1)


def test():
    print("---: Testing Projects")
    cmd = run("dotnet test", shell=True)
    if cmd.returncode != 0:
        print("---: Tests Failed!")
        exit(1)


restore()
build()

# If unit tests:
# test()

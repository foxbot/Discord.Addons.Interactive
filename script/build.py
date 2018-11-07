from subprocess import call
from os import environ as env


def restore():
    print("---: Restoring Packages")
    if call("dotnet restore", shell=True) != 0:
        print("---: Restore Failed!")
        exit(1)


def build():
    print("---: Building Projects")
    if call("dotnet build", shell=True) != 0:
        print("---: Build Failed!")
        exit(1)


def test():
    print("---: Testing Projects")
    if call("dotnet test", shell=True) != 0:
        print("---: Tests Failed!")
        exit(1)


restore()
build()

# If unit tests:
# test()

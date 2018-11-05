from subprocess import run
from os import environ as env


def pack(projects, version, tag):
    print("---: Packing projects...")
    for project in projects:
        print(f"---: Packing {project}")
        cmd = run(
            f"dotnet pack {project} /p:BuildNumber={version} /p:IsTagBuild={tag}",
            shell=True)

        if cmd.returncode != 0:
            print(f"---: Failed to pack {project}!")
            exit(1)
        print(f"---: Packed {project}.")


def push():
    # TODO: Deploy to NuGet
    pass


build = env.get('TRAVIS_BUILD_NUMBER', default='dev').rjust(3, '0')
tag = env.get('TRAVIS_TAG', default='') is not ''

projects = ["Discord.Addons.Interactive/Discord.Addons.Interactive.csproj"]

pack(projects, build, tag)

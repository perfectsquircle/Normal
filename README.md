# Toadstool

> A Tesla package.

## Features

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec rutrum ante. Duis in tincidunt felis. Vivamus feugiat mattis lacus quis mollis. Donec et ligula tempus nibh dignissim ullamcorper. Ut molestie placerat leo nec rutrum. Cras posuere porttitor risus, id tristique ligula lobortis eu. Vivamus nulla ipsum, blandit at massa at, venenatis hendrerit lectus. Pellentesque vel nunc tellus. Proin bibendum odio ac purus auctor consectetur.

## Installation

Sed elementum, tellus sit amet laoreet feugiat, ante lectus interdum est, at sagittis lacus nulla vitae mi. Nullam risus ante, sagittis nec pharetra vel, molestie sit amet dolor. Nam aliquam non nulla nec sollicitudin. Nam id congue nulla. Etiam nulla ante, feugiat in sapien bibendum, pulvinar feugiat purus. Mauris rhoncus mauris congue purus dictum molestie. Donec non felis vel turpis venenatis luctus. Duis maximus consequat pretium.

## Publishing

Update the version number in the csproj. Merges to master will create a release.

## Building

This project targets both .NET Standard 1.3 and .NET Framework 4.5. Because of this, you must have .NET Framework or Mono installed (in addition to .NET Core).

On macOS and Linux build environments, to build from .NET Core you must set the `FrameworkPathOverride` environment variable.

```bash
export FrameworkPathOverride=$(dirname $(which mono))/../lib/mono/4.5/
```

The `Dockerfile` installs Mono on top of the `dotnet:2.0-sdk` image, then sets `FrameworkPathOverride` to the known location of Mono.

See https://github.com/dotnet/sdk/issues/335

---

Built with &hearts; by [Trogdor](mailto:DL-DEPT-WEB-DEV@tesla.com?subject=Toadstool).

&copy; Tesla
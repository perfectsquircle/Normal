FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN apt-get update && \
    apt-get install -y make mono-devel && \
    apt-get clean

ENV FrameworkPathOverride /usr/lib/mono/4.5/

COPY . /app

WORKDIR /app

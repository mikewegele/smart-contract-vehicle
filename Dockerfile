FROM ubuntu:latest
LABEL authors="mikewegele"

ENTRYPOINT ["top", "-b"]
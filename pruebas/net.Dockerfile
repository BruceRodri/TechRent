FROM alpine:3.22
RUN apk add --no-cache iperf3 iproute2 curl
ENTRYPOINT ["/bin/sh", "-c"]
CMD ["sleep 36000"]

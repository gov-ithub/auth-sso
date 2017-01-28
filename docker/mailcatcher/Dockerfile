FROM schickling/mailcatcher

# smtp port
EXPOSE 1025

# webserver port
EXPOSE 1080

ENV MAILCATCHER_PORT_1025_TCP_ADDR='1025'

ENTRYPOINT ["mailcatcher", "--smtp-ip=0.0.0.0", "--http-ip=0.0.0.0", "--foreground"]
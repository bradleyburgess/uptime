package com.bradleyburgess.services;

import com.bradleyburgess.interfaces.IPinger;

public class HttpPinger implements IPinger {
    private final PingClient client;

    public HttpPinger(PingClient client) {
        this.client = client;
    }

    @Override
    public boolean ping() {
        try {
            int status = client.ping().getStatus();
            return (status >= 200 && status < 300) || status == 405;
        } catch (Exception e) {
            return false;
        }
    }
}

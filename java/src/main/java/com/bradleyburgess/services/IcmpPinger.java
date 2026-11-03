package com.bradleyburgess.services;

import java.net.InetAddress;

import com.bradleyburgess.interfaces.IPinger;

public class IcmpPinger implements IPinger {
    private final String host;

    public IcmpPinger(String host) {
        this.host = host;
    }

    @Override
    public boolean ping() {
        try {
            InetAddress address = InetAddress.getByName(host);
            var result = address.isReachable(2000);
            return result;
        } catch (Exception e) {
            return false;
        }
    }
}

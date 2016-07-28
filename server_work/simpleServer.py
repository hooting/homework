# -*- coding: utf-8 -*-

from common import conf
from network.simpleHost import SimpleHost
from server_template.service.dispatcher import Dispatcher
from service.login import LoginService

import time

current_milli_time = lambda: int(round(time.time() * 1000))

class SimpleServer(object):

    def __init__(self):
        super(SimpleServer, self).__init__()

        self.entities = {}
        self.host = SimpleHost()
        self.host.startup(50000)
        self.dispatcher = Dispatcher()

        self.initServices()
        return


    def generateEntityID(self):
        raise NotImplementedError

    def registerEntity(self, entity):
        eid = self.generateEntityID
        entity.id = eid

        self.entities[eid] = entity

        return

    def initServices(self):
        login_service = LoginService(conf.MSG_CS_LOGIN)
        self.dispatcher.register(conf.MSG_CS_LOGIN, login_service)

    def tick(self):
        self.host.process()

        while 1:
            event, owner, data = self.host.read()
            if event < 0: #所有事件处理完毕
                break
            elif event == conf.NET_CONNECTION_NEW:
                #TODO
                pass
            elif event == conf.NET_CONNECTION_LEAVE:
                #TODO
                pass
            elif event == conf.NET_CONNECTION_DATA:
                self.dispatcher.dispatch(data, owner)
        for eid, entity in self.entities.iteritems():
            # Note: you can not delete entity in tick.
            # you may cache delete items and delete in next frame
            # or just use items.
            entity.tick()

        return


if __name__ == "__main__":
    server = SimpleServer()

    current = current_milli_time()
    last = 0
    while 1:
        if current - last > 35:
            last = current
            server.tick()
        current = current_milli_time()




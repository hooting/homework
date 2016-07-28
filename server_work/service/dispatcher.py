# -*- coding: utf-8 -*-

import struct
from server_template.common import events

class Service(object):
    def __init__(self, sid = 0):
        super(Service, self).__init__()

        self.service_id = sid
        self.__command_map = {}

    def handle(self, msg, owner):
        cid = msg.cid
        if not cid in self.__command_map:
            raise Exception('bad command %s'%cid)

        f = self.__command_map[cid]
        return f(msg, owner)

    def register(self, cid, function):
        self.__command_map[cid] = function

    def registers(self, command_dict):
        self.__command_map = {}
        for cid in command_dict:
            self.register(cid, command_dict[cid])

class Dispatcher(object):
    def __init__(self):
        super(Dispatcher, self).__init__()

        self.__service_map = {}

    def getSID(self, raw):
        i = struct.calcsize('=H')
        record = struct.unpack('=H', raw[0:i])
        return record[0]

    def dispatch(self, data, owner):
        sid = self.getSID(data)
        msg = events.MSGS[sid]().unmarshal(data)
        if not sid in self.__service_map:
            raise Exception('bad service %d'%sid)

        svc = self.__service_map[sid]
        return svc.handle(msg, owner)

    def register(self, sid, svc):
        self.__service_map[sid] = svc

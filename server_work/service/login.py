# -*- coding: utf-8 -*-
# created by hooting on 2016/7/28

from dispatcher import Service
from server_template.common import conf

class LoginService(Service):
    def __init__(self, sid = 0):
        super(LoginService, self).__init__(sid)
        commands = {
            0x1001 : self.login,
            0x1002 : self.createUser,
        }
        self.registers(commands)

    def login(self, msg, owner):
        #TODO
        print "------"
        print msg.name
        print msg.pwd
        print "--------------"

    def createUser(self, msg, owner):
        #TODO
        print "create user"
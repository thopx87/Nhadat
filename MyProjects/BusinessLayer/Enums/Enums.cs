using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Enums
{
    public enum Actions
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        SEARCH = 4,
        LOGIN = 5,
        LOGOUT = 6,
        REGISTRY = 7,
        UPLOAD = 8,
        COMMENT = 9
    }

    public enum Errors
    {
        INPUT = -1,
        EXIST = -2,
        NOT_EXIST = -3,
        UNKNOWN = -4,
        BLOCK = -5,
        NOT_LOGIN = -6,
        ROLE_WRONG = -7,
        INSERT_ERROR = -8,
        UPDATE_ERROR = -9,
        DELETE_ERROR = -10,
        HACKER = -11,
        INSERT_OR_UPDATE_ERROR = -12
    }

    public enum ProductCase
    {
        OK = 1,
        NO = 0,
        INPUT = -1,
        EXIST = -2,
        NOT_EXISTS = -3,
        FORMAT = -4,
        LARGER = -5,
        SMALL = -6,
        OVER_CHANGE_COUNT = -7

    }
}

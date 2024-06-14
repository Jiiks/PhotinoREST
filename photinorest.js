class PhotinoRest {

    static DEBUG = true;

    static SENDMESSAGE;
    static RECEIVEMESSAGE;

    static QUEUE = {};

    static VERSION = "0.0.1";

    static METHODS = {
        GET: 0,
        POST: 1,
        PUT: 2,
        DELETE: 3
    };

    static log(msg) {
        console.log(`[PhotinoRest] ${msg}`);
    }

    static dbg(msg) {
        if(PhotinoRest.DEBUG) PhotinoRest.log(msg);
    }

    static setup() {
        PhotinoRest.RECEIVEMESSAGE(PhotinoRest.receiveMessage);
    }

    static receiveMessage(message) {
        console.log(`Message Recieve`);
        const parse = JSON.parse(message);
        const { time } = parse.headers;

        if(PhotinoRest.QUEUE[time] === undefined) return;

        PhotinoRest.QUEUE[time].resolve(parse);
        delete PhotinoRest.QUEUE[time];
    }

    static prepare(path, params, method, resolve, reject) {
        const time = Date.now().toString();
        this.QUEUE[time] = { resolve, reject };
        return JSON.stringify({
            headers: {
                time,
                version: PhotinoRest.VERSION,
                method
            },
            path,
            body: params
        });
    }

    static parse(res) {
        return JSON.parse(res);
    }

    static async GET(path, params) {
        PhotinoRest.dbg(`GET: ${path} | ${params}`);
        return new Promise((resolve, reject) => {
            PhotinoRest.SENDMESSAGE(PhotinoRest.prepare(path, params, PhotinoRest.METHODS.GET, resolve, reject));
        });
    }

    static async POST(path, params) {
        PhotinoRest.dbg(`POST: ${path} | ${params}`);
        return new Promise((resolve, reject) => {
            PhotinoRest.SENDMESSAGE(PhotinoRest.prepare(path, params, PhotinoRest.METHODS.POST, resolve, reject));
        });
    }

    static async PUT(path, params) {
        PhotinoRest.dbg(`PUT: ${path} | ${params}`);
        return new Promise((resolve, reject) => {
            PhotinoRest.SENDMESSAGE(PhotinoRest.prepare(path, params, PhotinoRest.METHODS.PUT, resolve, reject));
        });
    }

    static async DELETE(path, params) {
        PhotinoRest.dbg(`DELETE: ${path} | ${params}`);
        return new Promise((resolve, reject) => {
            PhotinoRest.SENDMESSAGE(PhotinoRest.prepare(path, params, PhotinoRest.METHODS.DELETE, resolve, reject));
        });
    }
}

PhotinoRest.SENDMESSAGE = window.external.sendMessage;
PhotinoRest.RECEIVEMESSAGE = window.external.receiveMessage;
PhotinoRest.setup();

window.PR = window.PhotinoRest = PhotinoRest;
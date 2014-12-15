var http = require('http');

function requestFactory(options, callback) {

    var cfg = options;
    if (typeof options === 'string') {
        cfg = { uri: options };
    }

    cfg.callback = callback || options.callback;

    return new requestFactory.Request(cfg);
}


function Request(options) {
    this.options = options;
    this.go();

}

Request.prototype.go = function () {
    var me = this;
    var strings = [];
    var buffer;

    me.req = http.request(me.options, function (res) {
        if (me.options.callback) {
            res.on('data', function (chunk) {
                var me = this;
                if (Buffer.isBuffer(chunk)) {
                    if (!buffer) {
                        buffer = new Buffer(1024);
                    }
                    //hack till i get the right bla
                    chunk.copy(buffer);
                } else {
                    strings.push(chunk);
                }
            });
            res.on('end', function () {
                me.onEnd(strings, buffer, res, me.options.callback);
            });
        }
    });

    me.req.end();
    //todo sedn data on req.

};


Request.prototype.onEnd = function (strings, buffer, response, callback) {
    var self = this;
    if (self._aborted) {
        return;
    }

    if (buffer.length) {
        if (self.encoding === null) {
            // response.body = buffer
            // can't move to this until https://github.com/rvagg/bl/issues/13
            response.body = buffer.slice();
        } else {
            response.body = buffer.asString(self.encoding);
        }
    } else if (strings.length) {
        // The UTF8 BOM [0xEF,0xBB,0xBF] is converted to [0xFE,0xFF] in the JS UTC16/UCS2 representation.
        // Strip this value out when the encoding is set to 'utf8', as upstream consumers won't expect it and it breaks JSON.parse().
        if (self.encoding === 'utf8' && strings[0].length > 0 && strings[0][0] === '\uFEFF') {
            strings[0] = strings[0].substring(1);
        }
        response.body = strings.join('');
    }

    if (self.options.json) {
        try {
            response.body = JSON.parse(response.body);
        } catch (e) { }
    }

    if (typeof response.body === 'undefined' && !self._json) {
        response.body = '';
    }
    callback.call(null, null, response, response.body);
};

if (typeof module != 'undefined' && module != null) {
    module.exports = request;
}

requestFactory.Request = Request;

request.exports = requestFactory;

function retrocycle(o) {
    var self = this;
    self.identifiers = [];
    self.refs = [];

    self.rez = function (value) {

        // The rez function walks recursively through the object looking for $ref
        // properties. When it finds one that has a value that is a path, then it
        // replaces the $ref object with a reference to the value that is found by
        // the path.

        var i, item, name, path;

        if (value && typeof value === 'object') {
            if (Object.prototype.toString.apply(value) === '[object Array]') {
                for (i = 0; i < value.length; i += 1) {
                    item = value[i];
                    if (item && typeof item === 'object') {
                        path = item.$ref;
                        if (typeof path === 'string' && path != null) {
                            //self.refs[parseInt(path)] = {};

                            value[i] = self.identifiers[parseInt(path)]
                        } else {
                            self.identifiers[parseInt(item.$id)] = item;
                            self.rez(item);
                        }
                    }
                }
            } else {
                for (name in value) {
                    if (typeof value[name] === 'object') {
                        item = value[name];
                        if (item) {
                            path = item.$ref;
                            if (typeof path === 'string' && path != null) {
                                value[name] = self.identifiers[parseInt(path)]
                            } else {
                                self.identifiers[parseInt(item.$id)] = item;
                                self.rez(item);
                            }
                        }
                    }
                }
            }
        }

    };
    self.rez(o);
    self.identifiers = [];
}
CodeMirror.defineMode('csharp', function () {
    var words = {
            'abstract': 'keyword',
            'as': 'keyword',
            'base': 'keyword',
            'bool': 'keyword',
            'break': 'keyword',
            'byte': 'keyword',
            'case': 'keyword',
            'catch': 'keyword',
            'char': 'keyword',
            'checked': 'keyword',
            'class': 'keyword',
            'const': 'keyword',
            'continue': 'keyword',
            'decimal': 'keyword',
            'default': 'keyword',
            'delegate': 'keyword',
            'do': 'keyword',
            'double': 'keyword',
            'else': 'keyword',
            'enum': 'keyword',
            'event': 'keyword',
            'explicit': 'keyword',
            'extern': 'keyword',
            'false': 'keyword',
            'finally': 'keyword',
            'fixed': 'keyword',
            'float': 'keyword',
            'for': 'keyword',
            'foreach': 'keyword',
            'goto': 'keyword',
            'if': 'keyword',
            'implicit': 'keyword',
            'in': 'keyword',
            'int': 'keyword',
            'interface': 'keyword',
            'internal': 'keyword',
            'is': 'keyword',
            'lock': 'keyword',
            'long': 'keyword',
            'namespace': 'keyword',
            'new': 'keyword',
            'null': 'keyword',
            'object': 'keyword',
            'operator': 'keyword',
            'out': 'keyword',
            'override': 'keyword',
            'params': 'keyword',
            'private': 'keyword',
            'protected': 'keyword',
            'public': 'keyword',
            'redonly': 'keyword',
            'ref': 'keyword',
            'sbyte': 'keyword',
            'sealed': 'keyword',
            'short': 'keyword',
            'sizeof': 'keyword',
            'stackalloc': 'keyword',
            'static': 'keyword',
            'string': 'keyword',
            'struct': 'keyword',
            'switch': 'keyword',
            'this': 'keyword',
            'throw': 'keyword',
            'true': 'keyword',
            'try': 'keyword',
            'typeof': 'keyword',
            'uint': 'keyword',
            'ulong': 'keyword',
            'unchecked': 'keyword',
            'unsafe': 'keyword',
            'ushort': 'keyword',
            'using': 'keyword',
            'var': 'keyword',
            'virtual': 'keyword',
            'void': 'keyword',
            'volatile': 'keyword',
            'while': 'keyword',
    };

    function tokenBase(stream, state) {
        var ch = stream.next();

        if (ch === '"') {
            state.tokenize = tokenString;
            return state.tokenize(stream, state);
        }
        if (ch === '/') {
            if (stream.eat('/')) {
                stream.skipToEnd();
                return 'comment';
            }
        }
        if (ch === '(') {
            if (stream.eat('*')) {
                state.commentLevel++;
                state.tokenize = tokenComment;
                return state.tokenize(stream, state);
            }
        }
        if (ch === '~') {
            stream.eatWhile(/\w/);
            return 'variable-2';
        }
        if (ch === '`') {
            stream.eatWhile(/\w/);
            return 'quote';
        }
        if (/\d/.test(ch)) {
            stream.eatWhile(/[\d]/);
            if (stream.eat('.')) {
                stream.eatWhile(/[\d]/);
            }
            return 'number';
        }
        if (/[+\-*&%=<>!?|]/.test(ch)) {
            return 'operator';
        }
        stream.eatWhile(/\w/);
        var cur = stream.current();
        return words[cur] || 'variable';
    }

    function tokenString(stream, state) {
        var next, end = false, escaped = false;
        while ((next = stream.next()) != null) {
            if (next === '"' && !escaped) {
                end = true;
                break;
            }
            escaped = !escaped && next === '\\';
        }
        if (end && !escaped) {
            state.tokenize = tokenBase;
        }
        return 'string';
    }

    function tokenComment(stream, state) {
        var prev, next;
        while (state.commentLevel > 0 && (next = stream.next()) != null) {
            if (prev === '(' && next === '*') state.commentLevel++;
            if (prev === '*' && next === ')') state.commentLevel--;
            prev = next;
        }
        if (state.commentLevel <= 0) {
            state.tokenize = tokenBase;
        }
        return 'comment';
    }

    return {
        startState: function () { return { tokenize: tokenBase, commentLevel: 0 }; },
        token: function (stream, state) {
            if (stream.eatSpace()) return null;
            return state.tokenize(stream, state);
        },

        blockCommentStart: "(*",
        blockCommentEnd: "*)",
        lineComment: '//'
    };
});

CodeMirror.defineMIME("text/x-csharp", "csharp");

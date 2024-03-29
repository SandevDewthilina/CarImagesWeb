'use strict';

Object.defineProperty(exports, '__esModule', {value: true});

var compilerDom = require('@vue/compiler-dom');
var runtimeDom = require('@vue/runtime-dom');
var shared = require('@vue/shared');

function _interopNamespace(e) {
    if (e && e.__esModule) return e;
    var n = Object.create(null);
    if (e) {
        Object.keys(e).forEach(function (k) {
            n[k] = e[k];
        });
    }
    n['default'] = e;
    return Object.freeze(n);
}

var runtimeDom__namespace = /*#__PURE__*/_interopNamespace(runtimeDom);

// This entry is the "full-build" that includes both the runtime
const compileCache = Object.create(null);

function compileToFunction(template, options) {
    if (!shared.isString(template)) {
        if (template.nodeType) {
            template = template.innerHTML;
        } else {
            return shared.NOOP;
        }
    }
    const key = template;
    const cached = compileCache[key];
    if (cached) {
        return cached;
    }
    if (template[0] === '#') {
        const el = document.querySelector(template);
        // __UNSAFE__
        // Reason: potential execution of JS expressions in in-DOM template.
        // The user must make sure the in-DOM template is trusted. If it's rendered
        // by the server, the template should not contain any user data.
        template = el ? el.innerHTML : ``;
    }
    const {code} = compilerDom.compile(template, shared.extend({
        hoistStatic: true,
        onError: undefined,
        onWarn: shared.NOOP
    }, options));
    // The wildcard import results in a huge object with every export
    // with keys that cannot be mangled, and can be quite heavy size-wise.
    // In the global build we know `Vue` is available globally so we can avoid
    // the wildcard object.
    const render = (new Function('Vue', code)(runtimeDom__namespace));
    render._rc = true;
    return (compileCache[key] = render);
}

runtimeDom.registerRuntimeCompiler(compileToFunction);

Object.keys(runtimeDom).forEach(function (k) {
    if (k !== 'default') exports[k] = runtimeDom[k];
});
exports.compile = compileToFunction;

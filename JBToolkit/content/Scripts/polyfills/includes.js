﻿https://unpkg.com/mdn-polyfills@5.19.0/Array.prototype.includes.js
Array.prototype.includes || (Array.prototype.includes = function (r) { if (null == this) throw new TypeError("Array.prototype.includes called on null or undefined"); var e = Object(this), n = parseInt(e.length, 10) || 0; if (0 === n) return !1; var t, o, i = parseInt(arguments[1], 10) || 0; for (0 <= i ? t = i : (t = n + i) < 0 && (t = 0); t < n;) { if (r === (o = e[t]) || r != r && o != o) return !0; t++ } return !1 });

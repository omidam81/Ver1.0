//182
function toDeg(e) {
    return 180 * e / Math.PI
}

function toRad(e) {
    return e / 180 * Math.PI
}

var ciede = {
    distance: function (e, t) {
        var a, i = {"l": e[0], "a": e[1], "b": e[2]}, o = {
            "l": t[0],
            "a": t[1],
            "b": t[2]
        }, s = 1, l = 1, u = 1, c = Math.sqrt(Math.pow(i.a, 2) + Math.pow(i.b, 2)), d = Math.sqrt(Math.pow(o.a, 2) + Math.pow(o.b, 2)), h = (c + d) / 2, p = Math.pow(h, 7), m = (1 - Math.sqrt(p / (p + Math.pow(25, 7)))) / 2, f = (1 + m) * i.a, g = (1 + m) * o.a, y = Math.sqrt(Math.pow(f, 2) + Math.pow(i.b, 2)), v = Math.sqrt(Math.pow(g, 2) + Math.pow(o.b, 2)), b = (toDeg(Math.atan2(i.b, f)) + 360) % 360, N = (toDeg(Math.atan2(o.b, g)) + 360) % 360, w = o.l - i.l, _ = v - y, T = (b - N).abs;
        a = y * v == 0 ? 0 : 180 >= T ? N - b : T > 180 && b >= N ? N - b + 360 : N - b - 360, a = 2 * Math.sqrt(y * v) * Math.sin(a * Math.PI / 360);
        var D, C = (i.l + o.l) / 2, E = (y + v) / 2;
        D = y * v == 0 ? 0 : 180 >= T ? (b + N) / 2 : T > 180 && 360 > b + N ? (b + N + 360) / 2 : (b + N - 360) / 2;
        var M = Math.pow(C - 50, 2), S = 1 + .015 * M / Math.sqrt(20 + M), k = 1 + .045 * E, R = 1 - .17 * Math.cos(toRad(D - 30)) + .24 * Math.cos(toRad(2 * D)) + .32 * Math.cos(toRad(3 * D + 6)) - .2 * Math.cos(toRad(4 * D - 63)), P = 1 + .015 * R * E, x = Math.pow((D - 275) / 25, 2), A = 30 * Math.exp(-x), O = Math.pow(E, 7), I = 2 * Math.sqrt(O / (O + Math.pow(25, 7))), L = -Math.sin(toRad(2 * A)) * I, F = w / (S * s), B = _ / (k * l), U = a / (P * u);
        return Math.sqrt(Math.pow(F, 2) + Math.pow(B, 2) + Math.pow(U, 2) + L * B * U)
    }
};

namespace Contracts

type Request =
    | Add of int * int

type Response =
    | Added of int
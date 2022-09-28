import os
import math
import base64
import sys


def rsa(message, public_key):

    public_key = string_to_tuples((base64_decode(bytearray(public_key, 'utf-8'))).decode("utf-8"))
    message = message.encode("utf-8")
    x = encrypt(message, public_key)
    x = base64_encode(x)
    print(x.decode("utf-8"))
    return x.decode("utf-8")


def encrypt(msg, public_key):

    n,e = public_key
    key_length = byte_size(n)
    b = basket(msg, key_length)
    x = bytes2int(b)
    encrypted = pow(x, e, n)
    return int2bytes(encrypted, key_length)


def decrypt(msg, private_key):

    d,n = private_key
    length = byte_size(n)
    encrypted = bytes2int(msg)
    decrypted = pow(encrypted, d, n)
    vote = int2bytes(decrypted, length)
    sep_id = vote.find(b'\x00', 2)
    return vote[sep_id + 1:]


def basket(msg, target_length):
    data = b''
    data_length = target_length - len(msg) - 3
    while len(data) < data_length:
        n = data_length - len(data)
        temp_data = os.urandom(n + 5)
        temp_data = temp_data.replace(b'\x00', b'')
        data = data + temp_data[:n]
    return b''.join([b'\x00\x02', data, b'\x00', msg])


def bytes2int(b):
    return int.from_bytes(b, 'big', signed=False)


def byte_size(num):
    if num == 0:
        return 1
    return ceil_div(num.bit_length(), 8)


def string_to_tuples(s):
    x = list()
    temp = s.split('-')
    for i in temp:
        t = int(i)
        x.append(t)
    return tuple(x)


def ceil_div(num, div):

    q, mod = divmod(num, div)
    if mod:
        q += 1
    return q


def base64_encode(x):
    return base64.b64encode(x)


def int2bytes(num, size=0):
    n_bytes = max(1, math.ceil(num.bit_length() / 8))
    if size > 0:
        return num.to_bytes(size, 'big')
    return num.to_bytes(n_bytes, 'big')


def base64_decode(x):
    return base64.b64decode(x)


def main():
    if len(sys.argv) != 3:
        print("error")
        return "error"
    message = sys.argv[1]
    public_key = sys.argv[2]
    rsa(message, public_key)

if __name__ == "__main__":
    main()

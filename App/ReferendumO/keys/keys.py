import random
import math
import os
import base64
import uuid
import time

import database
from datetime import datetime
import xml.etree.ElementTree as ET

import multiprocessing
from multiprocessing.connection import Connection


def isInt(s):
    try:
        int(s)
        return True
    except ValueError:
        return False


def key_to_db(xml, keyid):
    key_name = "key-" + str(keyid)
    database.insertKey(key_name, xml)


def add_key_to_db(keyid, d, key):
    temp = ""
    temp += '<?xml version="1.0" encoding="utf-8"?>'
    temp += '<key id="' + str(keyid) + '" version="1">'
    temp += '<creationDate>' + str(d) + '</creationDate>'
    temp += '<activationDate></activationDate>'
    temp += '<expirationDate></expirationDate>'
    temp += '<encryptedKey>'
    temp += str(key)
    temp += '</encryptedKey></key>'
    key_name = "key-" + str(keyid)
    database.insertKey(key_name, temp)


def read_key_xml(n='keys.xml'):
    t = ET.parse(n)
    for key in t.iter('key'):
        key_id = key.attrib.get('id')
        # print(key.tag, key.attrib.get('id'))
        for dateT in key:
            if str(dateT.tag).upper() == "creationDate".upper():
                datetemp = dateT.text
            for a in dateT:
                for b in a:
                    for c in b:
                        for d in c:
                            key = d.text
        add_key_to_db(key_id, datetemp, key)


def getUsedPublicKeyFromXml(Id):
    file = "UsedKeys/" + Id + ".xml"
    t = ET.parse(file)
    for key in t.iter('key'):
        for dateT in key:
            if dateT.tag == "encryptedKey":
                return dateT.text


def datetime_utc_now():
    return datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%S.%fZ")


def get_public_key_from_govkey_to_xml():
    f = open('storage.govkey', 'r')
    pub_temp = ""
    line = f.readline()
    while line:
        line = f.readline()  # public
        pub_temp = line.rstrip()  # delete '\n'
        key_to_xml(uuid.uuid4(), pub_temp, datetime_utc_now())
        line = f.readline()  # priv
        private_key = line.rstrip()  # delete '\n'
        line = f.readline()  # end
        line = f.readline()  # begin
    f = open('keys.xml', 'a')
    f.write('</keys>')


def used_keys_to_xml(keyid, xml):
    file = "UsedKeys/" + keyid + ".xml"
    if not os.path.isfile(file):
        f = open(file, 'a')
        f.write(xml)


def key_to_xml(keyid, key, d):
    if not os.path.isfile('keys.xml'):
        f = open('keys.xml', 'a')
        f.write('<?xml version="1.0" encoding="utf-8"?>')
        f.write('<keys>')
    else:
        f = open('keys.xml', 'a')
    f.write('<key id="' + str(keyid) + '" version="1">')
    f.write('<creationDate>' + str(d) + '</creationDate>')
    f.write('<activationDate></activationDate>')
    f.write('<expirationDate></expirationDate>')
    f.write('<expirationDate></expirationDate>')
    f.write(
        '<descriptor deserializerType="{deserializerType}"><descriptor><encryption algorithm="RSA4096" /><validation algorithm="" />')
    f.write('<enc:encryptedSecret decryptorType="{decryptorType}" xmlns:enc="...">')
    f.write('<encryptedKey><value>')
    f.write(key)
    f.write('</value></encryptedKey></enc:encryptedSecret></descriptor></descriptor></key>')


def prepare_keys():
    f = open('primes400.txt', 'r')
    line = True
    while line:
        try:
            line = f.readline().rstrip()  # p
            p = int(line)
            line = f.readline().rstrip()  # q
            q = int(line)
        except ValueError:
            return
        public_key, private_key = generate_key_from_storage(p, q)
        public_key = base64_encode(tuples_to_string(public_key).encode('utf-8'))
        private_key = base64_encode(tuples_to_string(private_key).encode('utf-8'))
        save_govkey(public_key.decode("utf-8"), private_key.decode("utf-8"))


def generate_key_from_storage(p, q):
    n = p * q
    phi = (p - 1) * (q - 1)
    e = chooseE(phi)
    d = pow(e, -1, phi)
    return (n, e), (d, n)


def save_govkey(public_key, private_key):
    f = open('storage.govkey', 'a')
    f.write("-----BEGIN KEY PAIR-----" + "\n")
    f.write(public_key)
    f.write("\n")
    f.write(private_key)
    f.write("\n")
    f.write("-----END KEY PAIR-----" + "\n")


def get_private_key_from_govkey(public_key):
    f = open('storage.govkey', 'r')
    pub_temp = ""
    line = f.readline()
    while line:
        line = f.readline()  # public
        pub_temp = line.rstrip()  # delete '\n'
        line = f.readline()  # priv
        private_key = line.rstrip()  # delete '\n'
        if pub_temp == public_key:
            return private_key
        line = f.readline()  # end
        line = f.readline()  # begin


def my_join(tpl):
    return ', '.join(x if isinstance(x, str) else my_join(x) for x in tpl)


def tuples_to_string(t):
    x = str(t[0])
    for i in range(1, len(t)):
        x += ('-' + str(t[i]))
    return x


def string_to_tuples(s):
    x = list()
    temp = s.split('-')
    for i in temp:
        t = int(i)
        x.append(t)
    return tuple(x)


def base64_encode(x):
    return base64.b64encode(x)


def base64_decode(x):
    return base64.b64decode(x)


def get_random_line(afile, default=None):
    line = default
    for i, aline in enumerate(afile, start=1):
        if random.randrange(i) == 0:
            line = aline
    return line


def load_random_prime_from_file():
    f = open('primes200.txt', 'r')
    line = get_random_line(f)
    print(line)


def load_random_pair_from_file():
    f = open('primes200.txt', 'r')
    line = get_random_line(f)
    temp = line.split(';')
    for i in temp:
        isPrime = test_miller_rabin(int(i), 56)
        if isPrime == False:
            print("FALSE")


def load_all_pair_from_file():
    f = open('primes200.txt', 'r')
    line = f.readline().split(';')
    for i in line:
        isPrime = test_miller_rabin(int(i), 1)
        if isPrime == False:
            print("FALSE")
    print(line)
    while line:
        line = f.readline().split(';')
        for i in line:
            isPrime = test_miller_rabin(int(i), 1)
            if isPrime == False:
                print("FALSE")
        print(line)


def save_to_file(p, q):
    f = open('primes200.txt', 'a')
    string = str(p) + ";"
    f.write(string)
    string = str(q)
    f.write(string)
    f.write("\n")
    f.close()


def save_to_file_times(t):
    f = open('times.txt', 'a')
    string = str(t)
    f.write(string)
    f.write("\n")
    f.close()


def pair_generator_of_prime_numbers(bits):
    diff = bits // 32
    allBits = bits * 2
    pBits = bits + diff
    qBits = bits - diff
    p = prime_number(pBits, 3)
    q = prime_number(qBits, 3)

    switch = False
    while (p * q).bit_length() != allBits:
        if switch:
            p = prime_number(pBits, 3)
        else:
            q = prime_number(qBits, 3)

        switch = not switch

    return p, q


def get_prime(bits, pipe: Connection):
    while True:
        n = random.getrandbits(bits - 1) * 2 + 1
        if test_miller_rabin(n, 2):
            pipe.send(n)
            return


def get_prime2(bits, pipe: Connection):
    n = random.getrandbits(bits)
    if n & 1:
        isPrime = test_miller_rabin(n, 2)
    else:
        isPrime = False
    while isPrime != True:
        n = random.getrandbits(bits)
        if n & 1:
            isPrime = test_miller_rabin(n, 2)
        else:
            isPrime = False
    pipe.send(n)
    return


def prime_number(bits, threads_number):
    (receiving_pipe, outgoing_pipe) = multiprocessing.Pipe(duplex=False)
    try:
        process = [multiprocessing.Process(target=get_prime, args=(bits, outgoing_pipe))
                   for _ in range(threads_number)]
        for p in process:
            p.start()

        prime_num = receiving_pipe.recv()
    finally:
        receiving_pipe.close()
        outgoing_pipe.close()

    for p in process:
        p.terminate()

    return prime_num


def chooseE(phi):
    e = random.randrange(1, phi)
    g = math.gcd(e, phi)
    while g != 1:
        e = random.randrange(1, phi)
        g = math.gcd(e, phi)
    return e


def generate_keys():
    p, q = pair_generator_of_prime_numbers(2048)
    n = p * q
    phi = (p - 1) * (q - 1)
    e = chooseE(phi)
    d = pow(e, -1, phi)

    return (n, e), (d, n)


def test_miller_rabin(p, n):
    if p == 2 or p == 3:
        return True
    if p % 2 == 0:
        return False
    d = p - 1
    s = 0
    while d % 2 == 0:
        s = s + 1
        d //= 2
    for i in range(n):
        a = random.randrange(2, p - 1)  # [2;p-2]
        x = pow(a, d, p)
        if x == 1 or x == p - 1:
            continue
        j = 1
        while j < s and x != p - 1:
            x = pow(x, 2, p)
            if x == 1:
                return False
            j = j + 1
        if x != p - 1:
            return False
    return True


def getPrivateKeyByPublicKeyId(keyId):
    n = 'keys.xml'
    t = ET.parse(n)
    for key in t.iter('key'):
        key_id = key.attrib.get('id')
        if key_id == keyId:
            # print(key.tag, key.attrib.get('id'))
            for dateT in key:
                if str(dateT.tag).upper() == "creationDate".upper():
                    datetemp = dateT.text
                for a in dateT:
                    for b in a:
                        for c in b:
                            for d in c:
                                key = d.text
        add_key_to_db(key_id, datetemp, key)


def primesGenerator(n):
    for i in range(0, n):
        num = test_miller_rabin(n, 56)
        # save_to_file(num)


def prepare_primes_set():
    for i in range(28):
        start_time = time.time()
        p, q = pair_generator_of_prime_numbers(2048)
        seconds = (time.time() - start_time)
        save_to_file(p, q)
        save_to_file_times(seconds)


def main():
    print("Generowanie kluczy")
    #public_key, private_key = generate_keys()
    #public_key = base64_encode(tuples_to_string(public_key).encode('utf-8'))
    #private_key = base64_encode(tuples_to_string(private_key).encode('utf-8'))

    # prepare_primes_set()
    #load_all_pair_from_file()
    #print(multiprocessing.cpu_count())
    #p, q = pair_generator_of_prime_numbers(128)

if __name__ == "__main__":
    main()

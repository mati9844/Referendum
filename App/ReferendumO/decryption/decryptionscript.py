import os
import math
import base64
import xml.etree.ElementTree as ET
from datetime import datetime

import database


def decrypt(msg, private_key):

    d,n = private_key
    length = byte_size(n)
    encrypted = bytes2int(msg)
    decrypted = pow(encrypted, d, n)
    vote = int2bytes(decrypted, length)
    sep_id = vote.find(b'\x00', 2)
    return vote[sep_id + 1:]


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


def used_keys_to_xml(keyid, xml):
    file = "UsedKeys/" + keyid + ".xml"
    if not os.path.isfile(file):
        f = open(file, 'a')
        f.write(xml)


def getUsedPublicKeyFromXml(Id):
    file = "UsedKeys/" + Id + ".xml"
    t = ET.parse(file)
    for key in t.iter('key'):
        for dateT in key:
            if dateT.tag == "encryptedKey":
                return dateT.text

def get_private_key_from_govkey(public_key):
    f = open('storage.govkey', 'r')
    pub_temp = ""
    line = f.readline()
    while line:
        line = f.readline() #public
        pub_temp = line.rstrip() #delete '\n'
        line = f.readline() #priv
        private_key = line.rstrip() #delete '\n'
        if (pub_temp == public_key):
            return private_key
        line = f.readline() #end
        line = f.readline() #begin


def datetime_utc_now():
    return datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%S.%fZ")


def result_protocol(vid, question, correctVotes, incorrectVotes, yes, no):
    if not os.path.isfile('result.xml'):
        f = open('result.xml', 'a')
        f.write('<?xml version="1.0" encoding="ISO-8859-2"?>')
        f.write('<results>')
    else:
        f = open('result.xml', 'a')
    f.write('<result id="' + str(vid) + '" version="1">')
    f.write('<date>' + str(datetime_utc_now()) + '</date>')
    f.write('<question>' + str(question) + '</question>')
    f.write('<positiveAnswers>' + str(yes) + '</positiveAnswers>')
    f.write('<negativeAnswers>' + str(no) + '</negativeAnswers>')
    f.write('<correctVotes>')
    counter = 0
    for i in correctVotes:
        f.write('<vote id="' + str(counter) + '">')
        f.write('<id>' + str(i) + '</id>')
        f.write('</vote>')
        counter += 1
    f.write('</correctVotes>')
    f.write('<incorrectVotes>')
    counter = 0
    for i in incorrectVotes:
        f.write('<vote id="' + str(counter) + '">')
        f.write('<id>' + str(i) + '</id>')
        f.write('</vote>')
        counter += 1
    f.write('</incorrectVotes>')
    f.write('</result>')
    f.write('</results>')


def main():

    yesCount = 0
    noCount = 0
    votesId = database.getCorrectVotesIdsList()
    incorrectVotesId = database.getIncorrectVotesIdsList()
    question = ()
    for Id in votesId:
        question = database.getVoteQuestion(Id)
        xml = database.getKeyToXml(Id)
        used_keys_to_xml(Id, xml)
        key = getUsedPublicKeyFromXml(Id)
        private_key = string_to_tuples(
            (base64_decode(bytearray(get_private_key_from_govkey(key), 'utf-8'))).decode("utf-8"))
        answer = database.getVote(Id)
        if answer != "error":
            answer = bytes(answer, 'utf-8')
            # print(answer)
            answer = base64_decode(answer)
            y = decrypt(answer, private_key)
            y = y.decode("utf-8")
            print(y)
            if y == "TAK":
                yesCount += 1
            if y == "NIE":
                noCount += 1
    print("WYNIKI")
    print("TAK: " + str(yesCount))
    print("NIE: " + str(noCount))
    tempQuestion = ""
    tempQuestion += question
    result_protocol(0, tempQuestion, votesId, incorrectVotesId, yesCount, noCount)


if __name__ == "__main__":
    main()

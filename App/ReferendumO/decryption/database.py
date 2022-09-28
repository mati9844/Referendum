import pyodbc


def insertKey(name, key):
    conn = connection('Keys')
    cursor = conn.cursor()
    cursor.execute("insert into dbo.DataProtectionKeys(FriendlyName, Xml) values (?, ?)", name, key)
    conn.commit()
    cursor.close()
    conn.close()


def getKeyToXml(Id):
    xml = ""
    Id = "key-"+str(Id)
    conn = connection('Keys')
    cursor = conn.cursor()
    cursor.execute("SELECT Xml FROM dbo.DataProtectionKeys WHERE FriendlyName = ?;", Id)
    row = cursor.fetchone()
    xml = row[0]
    cursor.close()
    conn.close()
    return xml


def getVote(voteId):
    conn = connection('WebApplication')
    cursor = conn.cursor()
    cursor.execute("SELECT Answer FROM dbo.Votes WHERE Id = ?;", voteId)
    row = cursor.fetchone()
    voteAnswer = row[0]
    cursor.close()
    conn.close()
    return voteAnswer


def getVoteQuestion(voteId):
    conn = connection('WebApplication')
    cursor = conn.cursor()
    cursor.execute("SELECT Question FROM dbo.Votes WHERE Id = ?;", voteId)
    row = cursor.fetchone()
    voteQuestion = row[0]
    cursor.close()
    conn.close()
    return voteQuestion


def getCorrectVotesIdsList():
    votesId = []
    conn = connection('WebApplication')
    cursor = conn.cursor()
    cursor.execute("SELECT VoteId FROM dbo.Envelopes WHERE Status=0;")
    row = cursor.fetchone()
    while row:
        votesId.append(row[0])
        row = cursor.fetchone()
    cursor.close()
    conn.close()
    return votesId


def getIncorrectVotesIdsList():
    votesId = []
    conn = connection('WebApplication')
    cursor = conn.cursor()
    cursor.execute("SELECT VoteId FROM dbo.Envelopes WHERE Status!=0;")
    row = cursor.fetchone()
    while row:
        votesId.append(row[0])
        row = cursor.fetchone()
    cursor.close()
    conn.close()
    return votesId


def connection(db):
    conn = pyodbc.connect('Driver={SQL Server};'
                          'Server=LAPTOP-RKPJVVF0;'
                          'Database='+db+';'
                          'Trusted_Connection=yes;')

    return conn

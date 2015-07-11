# Eric Gan
# Parses the XML file ESV.xml. Creates a new file esv.txt containing
# the plaintext.

import xml.etree.ElementTree as ET
import sys

if __name__ == '__main__':
    if len(sys.argv) != 3: 
        print "Usage: %s <inFile> <outFile>" % (sys.argv[0])
        exit(0)

    inFile = sys.argv[1]
    outFile = sys.argv[2]

    tree = ET.parse(inFile)
    bibleRoot = tree.getroot()
   
    bible = dict()
    for book in bibleRoot:
        bookDict = dict()
        for chapter in book:
            corpus = ""
            for verse in chapter: 
                corpus += verse.text + " "

            bookDict[chapter.get("n")] = corpus
        bible[book.get("n")] = bookDict

    with open(outFile, "w") as outputfd: 
        outputfd.write(repr(bible))

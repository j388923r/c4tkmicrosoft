# Eric Gan
# Parses the XML file ESV.xml. Creates a new file esv.txt containing
# the plaintext.

import xml.etree.ElementTree as ET
from nltk.corpus import stopwords
from nltk.stem.lancaster import LancasterStemmer
from nltk.tokenize import RegexpTokenizer

if __name__ == '__main__':
    tree = ET.parse("ESV_processed.xml")
    bibleRoot = tree.getroot()
    st = LancasterStemmer()
    tokenizer = RegexpTokenizer(r'\w+')
   
    for book in bibleRoot:
        print book.get("n")
        for chapter in book:
            for verse in chapter: 
                newTextArr = filter(
                    lambda w: w.lower() not in stopwords.words("english"), 
                    tokenizer.tokenize(verse.text))
                stemmedTextArr = map(
                    lambda w: st.stem(w), 
                    newTextArr)
                verse.text = " ".join(stemmedTextArr)
                verse.set("updated", "yes")

    tree.write("ESV_processed.xml")
    print "Done!"

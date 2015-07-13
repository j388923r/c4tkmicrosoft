import xml.etree.ElementTree as ET
import nltk
from nltk.corpus import stopwords
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.decomposition import NMF
from stemming.porter2 import stem
import numpy as np
import lda
import time


if __name__ == '__main__':
    tree = ET.parse("ESV_processed.xml")
    bible = tree.getroot()
   
    # Parse XML into array of bible chapters
    #bible_arr = dict()
    array = []
    for book in bible:	
        print(book.get("n"))
        #chapter_arr = dict()
        for chapter in book:
            corpus = ""
            for verse in chapter:
                corpus += verse.text + " "

            # Now doing this in XML
            #filtered_corpus = [word for word in corpus if word.lower() not in stopwords.words('english')]
            #filtered_corpus = " ".join(filtered_corpus)

            # Add the chapter number as a key for its text    
            #chapter_arr[chapter.get("n")] = filtered_corpus
            array.append(corpus)

        # Add the Book name as key for the chapter dictionary
        # Enables book lookup such as: bible_arr["Genesis"]["1"]
        #bible_arr[book.get("n")] = chapter_arr

    #print(bible_arr["Genesis"]["1"])

    # Unigram (one word = one feature)
    vectorizer = TfidfVectorizer(ngram_range = (1,1))
    dt_matrix = vectorizer.fit_transform(array)
    td_matrix = np.transpose(dt_matrix)
    td_matrix = td_matrix.dot(5000)
    nmf = NMF(n_components=50, init='random', random_state=0, sparseness='components')
    W = nmf.fit_transform(td_matrix)
    H = nmf.components_

    # Write W and H to .txt files
    w_output = open("w_nmf.csv", "w")
    h_output = open("h_nmf.csv", "w")

    for row in W:
        for element in row:
            w_output.write(str(element) + ",")
        w_output.write("\n")
    w_output.close()

    for row in H:
        for element in row:
            h_output.write(str(element) + ",")
        h_output.write("\n")
    h_output.close()
    print(W.shape)
    print(H.shape)        

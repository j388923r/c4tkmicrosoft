
from sklearn.feature_extraction.text import TfidfVectorizer
#can add or just use bigrams as well
#ngram range specifies what ngrams you want
#use (2,2) to use just bigrams
#use (1,1) to use just unigrams
#use (1,2) to use both unigrams and bigrams

vectorizer = TfidfVectorizer(ngram_range = (1,1))
tfidf = vectorizer.fit_transform(corpus)
nmf_input = tfidf.toarray()



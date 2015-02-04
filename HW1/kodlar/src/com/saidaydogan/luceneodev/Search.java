package com.saidaydogan.luceneodev;

import java.io.File;
import org.apache.lucene.analysis.Analyzer;
import org.apache.lucene.analysis.standard.StandardAnalyzer;
import org.apache.lucene.document.Document;
import org.apache.lucene.index.DirectoryReader;
import org.apache.lucene.index.IndexReader;
import org.apache.lucene.queryparser.classic.QueryParser;
import org.apache.lucene.search.IndexSearcher;
import org.apache.lucene.search.Query;
import org.apache.lucene.search.ScoreDoc;
import org.apache.lucene.search.TopDocs;
import org.apache.lucene.store.FSDirectory;
import org.apache.lucene.util.Version;

/** Simple command-line based search demo. */
public class Search {

	private Search() {
	}

	public static void SearchQuery(String queryString) throws Exception {

		// index dosyalarinin okunacagi klasor adi
		String index = "index";
		// arama yapilacak alan (index olusturulurken haber icerikleri
		// contents'e atandigi icin contents kullanildi
		String field = "contents";

		// Index dosyasi okunuyor
		IndexReader reader = DirectoryReader.open(FSDirectory.open(new File(index)));
		IndexSearcher searcher = new IndexSearcher(reader);

		// Sorgu lucene icin ayarlaniyor
		Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_47);
		QueryParser parser = new QueryParser(Version.LUCENE_47, field, analyzer);	
		Query query = parser.parse(queryString);
		
		// Arama yaptigimiz sorgu belirtiliyor
		System.out.println("Arama yapilan sorgu: " + query.toString(field));

		// Arama islemi yapiliyor
		TopDocs results = searcher.search(query, 100);
		// Lucenenin hesapladigi skorlara erisiliyor
		ScoreDoc[] hits = results.scoreDocs;

		// Kayit sayisi
		int numTotalHits = results.totalHits;
		System.out.println(numTotalHits + " adet kayit bulundu");
		Document doc;
		String path;
		// Kayitlarin Id, Skor ve Yolu yazdiriliyor
		for (int i = 0; i < numTotalHits; i++) {
			{
				doc = searcher.doc(hits[i].doc);
				path = doc.get("path");

				System.out.println("Id=" + hits[i].doc + " Skor="
						+ hits[i].score + " Yol= " + path);
			}
		}

		reader.close();
	}
}

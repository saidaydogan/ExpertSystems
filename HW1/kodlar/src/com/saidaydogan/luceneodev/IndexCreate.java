package com.saidaydogan.luceneodev;

import org.apache.lucene.analysis.Analyzer;
import org.apache.lucene.analysis.standard.StandardAnalyzer;
import org.apache.lucene.document.Document;
import org.apache.lucene.document.Field;
import org.apache.lucene.document.StringField;
import org.apache.lucene.document.TextField;
import org.apache.lucene.index.IndexWriter;
import org.apache.lucene.index.IndexWriterConfig.OpenMode;
import org.apache.lucene.index.IndexWriterConfig;
import org.apache.lucene.store.Directory;
import org.apache.lucene.store.FSDirectory;
import org.apache.lucene.util.Version;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;

public class IndexCreate {

	private IndexCreate() {
	}

	public static void CreateIndex(String indexPath, String sourceDirectory) {

		// Haberlerin oldugu klasoru veriyoruz
		String docsPath = sourceDirectory; // "C:\\Users\\Said\\Desktop\\ytü\\2014 bahar\\Uzman Sistem\\ödev1\\42bin_haber\\42bin_haber\\news";

		// Klasore erisilebilinirligi kontrol ediliyor
		final File file = new File(docsPath);
		if (!file.exists() || !file.canRead()) {
			System.out.println("Belirtilen klasor bulunamadý ya da açýlamadý");
			System.exit(1);
		}

		try {
			// Index dosyalarinin bulunacagi klasor olusturuluyor
			Directory dir = FSDirectory.open(new File(indexPath));
			Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_47);
			// Lucene index yazmak uzere ayarlaniyor
			IndexWriterConfig iwc = new IndexWriterConfig(Version.LUCENE_47,
					analyzer);
			iwc.setOpenMode(OpenMode.CREATE);
			IndexWriter writer = new IndexWriter(dir, iwc);
			FileInputStream fis;
			// Klasorun icindeki her dosyanin indexi olusturuluyor
			if (file.canRead()) {
				File[] folderList = file.listFiles();
				for (int a = 0; a < folderList.length; a++) {
					File[] fileList = folderList[a].listFiles();
					if (fileList != null) {
						for (int i = 0; i < fileList.length; i++) {

							fis = new FileInputStream(fileList[i]);

							try {

								Document doc = new Document();
								// index'e path adinda dosyanýn yolu ekleniyor
								Field pathField = new StringField("path",
										fileList[i].getPath(), Field.Store.YES);
								doc.add(pathField);

								// index'e contents adinda haber icerigi
								// ekleniyor
								doc.add(new TextField("contents",
										new BufferedReader(
												new InputStreamReader(fis,
														"UTF-8"))));

								System.out.println(fileList[i] + " eklendi");
								writer.addDocument(doc);

							} finally {
								fis.close();
							}
						}
					}
				}
			}
			writer.close();

		} catch (IOException e) {
			System.out.println(e.getMessage());
		}
	}
}

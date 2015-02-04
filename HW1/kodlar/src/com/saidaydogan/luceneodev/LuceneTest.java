package com.saidaydogan.luceneodev;

public class LuceneTest {
  
  private LuceneTest() {}
  
  public static void main(String[] args) throws Exception {
	  IndexCreate.CreateIndex("index", "42bin_haber\\42bin_haber\\news");
	  Search.SearchQuery("seven AND  sever");
  
  }
}




























































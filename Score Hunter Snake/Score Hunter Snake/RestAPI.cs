using System;
using System.Collections.Generic;
using System.Configuration;

using RestSharp;

namespace Score_Hunter_Snake {
    class RestAPI {

        String URL = ConfigurationManager.AppSettings["RestUrl"];
        String ROUTE = "index.php?A="+ConfigurationManager.AppSettings["RestAuth"];

        public bool SaveData(string name, int score, bool gameMode) {
            var client = new RestClient(URL);
            var request = new RestRequest(ROUTE + "&M=" + (gameMode ? "c" : "sh"), Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Scoreboard {
                Name = name,
                Score = score
            });

            IRestResponse response = client.Execute(request);
            if (response.Content.Contains("WRONG AUTH INFO"))
                return false;

            if (response.Content == "true")
                return true;

            return false;
        }

        public List<Scoreboard> LoadData(bool gameMode) {
            var client = new RestClient(URL);
            var request = new RestRequest(ROUTE + "&M=" + (gameMode ? "c" : "sh"), Method.GET);
            IRestResponse<Scoreboard> response = client.Execute<Scoreboard>(request);
            if (!response.Content.Contains("WRONG AUTH INFO")) {
                if (response.Content != "[]") {
                    List<Scoreboard> scores = new List<Scoreboard>();
                    var content = response.Content.Replace("},{", "|").Split('|');
                    for (int i = 0; i < content.Length; i++) {
                        string tempName = content[i].Split(',')[0].Split(':')[1].Replace("\"", "");
                        int tempScore = int.Parse(content[i].Split(',')[2].Split(':')[1].Replace("\"", ""));
                        scores.Add(new Scoreboard() { Place = (i + 1), Name = tempName, Score = tempScore });
                    }
                    return scores;
                }
            }
            return new List<Scoreboard>();
        }
    }

    class Scoreboard {

        public decimal Place { get; set; }
        public string Name { get; set; }
        public decimal Score { get; set; }
    }
}

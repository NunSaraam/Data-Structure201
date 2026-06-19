using System.Collections.Generic;
using UnityEngine;
using System.IO;


//딕셔너리 : 지하철역은 전체 역의 개수에 비해 각 역마다 연결된 이웃 역의 수가 적으니. 2차원 배열을 사용하여 연결되지 않은 역들의 정보까지 저장하면 메모리 낭비가 심하지만, 딕셔너리를 쓰면 실제로 연결된 역과 소요 시간만 저장하므로 메모리를 효율적으로 쓸 수 있어서.
//리스트 사용 : 직관적인 List를 순회하며 최솟값을 찾는 방식으로도 시간 초과 없이 충분히 빠르고 간결하게 구현할 수 있을것 같아서 리스트 방식을 사용함.


public class SubwayPathfinder : MonoBehaviour
{
    [Header("역 이름")]
    public string startStation = "동대문";
    public string endStation = "신설동";

    [Header("환승 패널티")]
    public int transferTimeMinutes = 5;

    // graph[출발역][도착역] = 소요시간
    private Dictionary<string, Dictionary<string, int>> graph = new Dictionary<string, Dictionary<string, int>>();

    // stationNodes = 몇호선인지
    private Dictionary<string, List<string>> stationNodes = new Dictionary<string, List<string>>();
    void Start()
    {
        LoadData();
        FindPath(startStation, endStation);
    }

    private void LoadData()
    {
        string path = Path.Combine(Application.dataPath, "subway_data.csv");

        if (!File.Exists(path)) return;

        string[] lines = File.ReadAllLines(path);

        string prevNode = null;
        string prevLine = null;

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            if (data.Length < 4) continue;

            string currentLine = data[1].Trim();
            string currentStation = data[2].Trim();

            // 호선 정보를 포함하여 독립된 노드 이름 생성 (예: "강남_2호선")
            string currentNode = $"{currentStation}_{currentLine}";

            // 그래프 초기화
            if (!graph.ContainsKey(currentNode)) graph[currentNode] = new Dictionary<string, int>();

            // 역 이름으로 호선들을 찾을 수 있게 분류표에 등록
            if (!stationNodes.ContainsKey(currentStation)) stationNodes[currentStation] = new List<string>();
            if (!stationNodes[currentStation].Contains(currentNode)) stationNodes[currentStation].Add(currentNode);

            int timeInSeconds = 0;
            string[] timeParts = data[3].Trim().Split(':');
            if (timeParts.Length == 2)
            {
                int.TryParse(timeParts[0], out int m);
                int.TryParse(timeParts[1], out int s);
                timeInSeconds = (m * 60) + s;
            }

            // 같은 호선으로 이어지는 순차적 역들만 선로 연결
            if (prevNode != null && prevLine == currentLine && timeInSeconds > 0)
            {
                graph[prevNode][currentNode] = timeInSeconds;
                graph[currentNode][prevNode] = timeInSeconds;
            }

            prevNode = currentNode;
            prevLine = currentLine;
        }

        // 환승역 간선길 만들기
        int transferPenaltySeconds = transferTimeMinutes * 60; // 분을 초로 변환

        foreach (var kvp in stationNodes)
        {
            List<string> linesInStation = kvp.Value;

            // 해당 역에 다니는 호선이 2개 이상이라면
            if (linesInStation.Count > 1)
            {
                // 호선들끼리 서로 갈아탈 수 있게
                for (int i = 0; i < linesInStation.Count; i++)
                {
                    for (int j = i + 1; j < linesInStation.Count; j++)
                    {
                        graph[linesInStation[i]][linesInStation[j]] = transferPenaltySeconds;
                        graph[linesInStation[j]][linesInStation[i]] = transferPenaltySeconds;
                    }
                }
            }
        }
    }

    private void FindPath(string start, string end)
    {
        if (!stationNodes.ContainsKey(start) || !stationNodes.ContainsKey(end))
        {
            Debug.LogError($"입력하신 '{start}' 또는 '{end}' 역이 존재하지 않습니다.");
            return;
        }

        Dictionary<string, int> dist = new Dictionary<string, int>();
        Dictionary<string, string> prev = new Dictionary<string, string>();
        List<string> unvisited = new List<string>();

        foreach (string node in graph.Keys)
        {
            dist[node] = 999999;
            prev[node] = null;
            unvisited.Add(node);
        }

        // 출발역의 모든 호선에서 0초로 탐색 시작
        foreach (string startNode in stationNodes[start])
        {
            dist[startNode] = 0;
        }

        string finalDestNode = null; // 도착한 최종 역_호선 저장

        while (unvisited.Count > 0)
        {
            string current = null;
            int minDist = 999999;

            foreach (string node in unvisited)
            {
                if (dist[node] < minDist)
                {
                    minDist = dist[node];
                    current = node;
                }
            }

            if (current == null) break;

            // 현재 도착한 역 이름이 목적지와 같다면 탐색 종료
            string currentStationName = current.Split('_')[0];
            if (currentStationName == end)
            {
                finalDestNode = current;
                break;
            }

            unvisited.Remove(current);

            foreach (var neighbor in graph[current])
            {
                string nextNode = neighbor.Key;
                int weight = neighbor.Value;
                int altDist = dist[current] + weight;

                if (altDist < dist[nextNode])
                {
                    dist[nextNode] = altDist;
                    prev[nextNode] = current;
                }
            }
        }

        if (finalDestNode == null)
        {
            Debug.Log("경로를 찾을 수 없습니다.");
            return;
        }

        // 경로 역추적
        List<string> pathList = new List<string>();
        string step = finalDestNode;

        while (step != null)
        {
            // 강남_2호선 텍스트를 "강남(2호선)" 형태로 변환 후 저장
            string[] parts = step.Split('_');
            string prettyName = $"{parts[0]}({parts[1]})";
            pathList.Add(prettyName);

            step = prev[step];
        }

        pathList.Reverse();

        int totalMinutes = dist[finalDestNode] / 60;

        string result =
            $"출발역을 입력하세요: {start}\n" +
            $"도착역을 입력하세요: {end}\n\n" +
            "=========================================\n" +
            "[조회 결과]\n" +
            $"이동 경로: {string.Join(" -> ", pathList)}\n" +
            $"총 소요 시간: {totalMinutes}분 (환승 {transferTimeMinutes}분 포함)\n" +
            "=========================================";

        Debug.Log(result);
    }
}

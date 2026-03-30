using System.Text.Json;
using System.IO;
using Godot;

namespace MapChat
{
    /// <summary>
    /// 모드 설정을 저장하고 불러오는 클래스입니다.
    /// MapShapesConfig.json처럼 별도 JSON 파일로 관리합니다.
    /// </summary>
    public class ChatConfig
    {
        // 채팅창을 여는 단축키 (기본값: F6)
        // MapShapes가 F5를 쓰므로 F6으로 설정
        public string Hotkey { get; set; } = "F6";
        
        // 채팅 텍스트 크기
        public int FontSize { get; set; } = 24;
        
        // 채팅 텍스트 색상 (RGBA)
        public string TextColor { get; set; } = "#FFFFFF";
        
        // 한 번에 표시할 수 있는 최대 채팅 수
        public int MaxMessages { get; set; } = 5;
        
        // 채팅이 화면에 표시되는 시간 (초)
        public float DisplayDuration { get; set; } = 10.0f;

        // 설정 파일 경로
        private static readonly string ConfigPath = "MapChatConfig.json";

        /// <summary>
        /// 설정 파일을 불러옵니다.
        /// 파일이 없으면 기본값을 사용합니다.
        /// </summary>
        public static ChatConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<ChatConfig>(json) ?? new ChatConfig();
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"[MapChat] 설정 파일 로드 실패: {e.Message}");
            }
            
            return new ChatConfig(); // 기본값 반환
        }

        /// <summary>
        /// 현재 설정을 파일에 저장합니다.
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception e)
            {
                GD.PrintErr($"[MapChat] 설정 파일 저장 실패: {e.Message}");
            }
        }
    }
}

public class AutoResponse
{
    public static string GetResponse(float score)
    {
        if (score >= 0.5f)
        {
            return "정말 기쁘군요! 당신의 긍정적인 에너지가 주변에도 전해질 거예요.";
        }
        else if (score >= 0.1f)
        {
            return "좋은 소식이네요! 계속해서 좋은 일들이 있길 바랍니다.";
        }
        else if (score > -0.1f)
        {
            return "그저 그런 하루였나 보군요. 내일은 더 나은 날이 될 거예요.";
        }
        else if (score > -0.5f)
        {
            return "힘든 일이 있었나 보네요. 괜찮아요, 모두가 그런 날이 있답니다.";
        }
        else
        {
            return "정말 힘든 시간을 보내고 있군요. 필요하다면 주변 사람들에게 도움을 요청하는 것도 좋아요.";
        }
    }
}


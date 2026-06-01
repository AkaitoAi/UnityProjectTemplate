using UnityEngine;

namespace AkaitoAi
{
    [CreateAssetMenu(fileName = "PlayerProfile", menuName = "ScriptableObjects/PlayerProfile", order = 1)]
    public class PlayerProfileSO : ScriptableObject
    {
        public SpriteArray[] profileDetails;

        private const string userDetailsPref = "User_Avatar_Gender_Country";
        private const string userNameAgePref = "User_Name_Age";

        [System.Serializable]
        public struct SpriteArray
        {
            public Sprite[] sprites;
        }

        public string GetString(int index)
        {
            return PlayerPrefs.GetString(userNameAgePref + index).ToString();
        }

        public Sprite GetSprite(int index)
        {
            return profileDetails[index].sprites[PlayerPrefs.GetInt(userDetailsPref + index)];
        }
    }
}

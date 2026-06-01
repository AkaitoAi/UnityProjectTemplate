using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AkaitoAi
{
    public class UpdateProfile : MonoBehaviour
    {
        [SerializeField] private Image avatar;
        [SerializeField] private Image flag;
        [SerializeField] private Image progressFiller;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text ageText;

        [SerializeField] private PlayerProfileSO profileSO;

        [ContextMenu("Update Player Profile")]
        public void UpdatePlayerProfile()
        {
            if (avatar != null) avatar.sprite = profileSO.GetSprite(0);
            if (flag != null) flag.sprite = profileSO.GetSprite(1);

            if (nameText != null) nameText.text = profileSO.GetString(0);
            if (ageText != null) ageText.text = "Age: " + profileSO.GetString(1);
        }

        private void OnEnable()
        {
            PlayerProfile.OnProfileUpdated += UpdatePlayerProfile;
        }

        private void OnDisable()
        {
            PlayerProfile.OnProfileUpdated -= UpdatePlayerProfile;
        }
    }
}

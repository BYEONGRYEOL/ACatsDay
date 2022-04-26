using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
namespace Isometric.UI
{

    public class UI_MainMenu : UI_Scene
    {
        //���θ޴� UI�� �����־���� Item�� enum���� �̸� ����,
        enum Buttons
        {
            Btn_Option,
            Btn_Score,
            Btn_Play
        }
        enum TextMeshPro
        {
            Tmpro_Label
        }

        private void Awake()
        {
            Init();
        }
        protected override void Init()
        {
            base.Init();

            // UI���Ͽ� ��ư�� Tmpro�� ������ Dictionary�� �޾� �˰��ֵ���
            Bind<Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(TextMeshPro));

            //Text ����
            GetText((int)TextMeshPro.Tmpro_Label).GetComponent<TextMeshProUGUI>().text = "A Cats Day";

            //Event ����
            BindEvent(GetButton((int)Buttons.Btn_Play).gameObject, PointerEventData 
                => { Managers.Scene.LoadScene(Enums.Scene.SceneGame); });
        }
    }

}
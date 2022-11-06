using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSContoller : MonoBehaviour
{
    // �ړ��p�̕ϐ�
    float x, z;
    // �X�s�[�h�����p�ϐ�
    float speed = 0.1f;

    // �J�����I�u�W�F�N�g�p�̕ϐ�
    public GameObject cam;
    // �p�x�������ϐ�
    Quaternion cameraRot, characterRot;
    // �}�E�X�̊��x�����p
    float Xsensityvity = 3f, Ysensityvity = 3f;
    //�}�E�X�̕\���؂�ւ�
    bool cursorLock = true;
    // �p�x�̐���
    float minX = -90f, maxX = 90f;

    public Animator animator;

    // �����e��A�ō������e��A�}�K�W�����e���A�}�K�W�����ő吔
    int ammuniton = 50, maxAmmuniton = 50, ammoClip = 10, maxAmmoClip = 10;

    // �̗́AMax�̗́A�̗�ber�A�e��e�L�X�g
    int playerHP = 100, maxPlayerHP = 100;
    public Slider hpBer;
    public Text ammoText;


    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        GameState.canShoot = true;

        hpBer.value = playerHP;
        ammoText.text = ammoClip + "/" + ammuniton;
    }

    // Update is called once per frame
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock();

        // �ˌ�
        if (Input.GetMouseButton(0) && GameState.canShoot)
        {
            if (ammoClip > 0)
            {
                animator.SetTrigger("Fire");
                GameState.canShoot = false;

                ammoClip--; // �e������炷
                ammoText.text = ammoClip + "/" + ammuniton;
            }
            else
            {
                Debug.Log("�e�؂�");
            }
            
        }

        // �����[�h
        if (Input.GetKeyDown(KeyCode.R))
        {
            int amountNeed = maxAmmoClip - ammoClip;
            int ammoAvailable = amountNeed < ammuniton ? amountNeed : ammuniton;

            if(amountNeed != 0 && ammuniton != 0)
            {
                animator.SetTrigger("Reload");
                ammuniton -= ammoAvailable;
                ammoClip += ammoAvailable;
                ammoText.text = ammoClip + "/" + ammuniton;
            }
            
        }

        // �ړ�
        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if (!animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", true);
            }
        }
        else if (animator.GetBool("Walk"))
        {
            animator.SetBool("Walk", false);
        }

        // ����
        if (z > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (!animator.GetBool("Run"))
            {
                animator.SetBool("Run", true);
                speed = 0.25f;
            }
        }
        else if (animator.GetBool("Run"))
        {
            animator.SetBool("Run", false);
            speed = 0.1f;
        }

    }






    // 0.2sec���ɌĂ΂�郁�\�b�h
    private void FixedUpdate()
    {
        x = 0;
        z = 0;
        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x, 0, z);
        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }




    /*
     * �J�[�\���̕\�����Ǘ�����
     */
    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }
}

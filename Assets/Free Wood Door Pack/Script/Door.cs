using System.Collections;
using UnityEngine;

namespace DoorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open;
        public float smooth = 1.0f;
        public float DoorOpenAngle = -90.0f;
        public float DoorCloseAngle = 0.0f;
        public AudioSource asource;
        public AudioClip openDoor, closeDoor;

        void Start()
        {
            asource = GetComponent<AudioSource>();
            if (asource == null)
                Debug.LogError("AudioSource missing on Door!");
        }

        void Update()
        {
            Quaternion target = open ? Quaternion.Euler(0, DoorOpenAngle, 0) : Quaternion.Euler(0, DoorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
        }

        public void OpenDoor()
        {
            open = !open;
            asource.clip = open ? openDoor : closeDoor;
            asource.Play();
        }
    }
}
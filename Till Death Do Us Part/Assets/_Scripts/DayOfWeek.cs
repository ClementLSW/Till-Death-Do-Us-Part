using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayOfWeek : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public enum Day
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7,
    }
    [SerializeField] Image monday, tuesday, wednesday, thursday, friday, saturday, sunday;

    void HideAllDayImages()
    {
        monday.enabled = false;
        tuesday.enabled = false;
        wednesday.enabled = false;
        thursday.enabled = false;
        friday.enabled = false;
        saturday.enabled = false;
        sunday.enabled = false;
    }

    public IEnumerator DisplayDayOfWeek(Day dayToDisplay)
    {
        HideAllDayImages();
        switch (dayToDisplay)
        {
            case Day.Monday:
                monday.enabled = true;
                break;
            case Day.Tuesday:
                tuesday.enabled = true;
                break;
            case Day.Wednesday:
                wednesday.enabled = true;
                break;
            case Day.Thursday:
                thursday.enabled = true;
                break;
            case Day.Friday:
                friday.enabled = true;
                break;
            case Day.Saturday:
                saturday.enabled = true;
                break;
            case Day.Sunday:
                sunday.enabled = true;
                break;
        }
        animator.Play("Day Change");
        yield return new WaitForSeconds(3);
    }
}

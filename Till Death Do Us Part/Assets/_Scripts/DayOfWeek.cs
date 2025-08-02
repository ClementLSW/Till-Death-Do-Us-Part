using System;
using System.Collections;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class DayOfWeek : MonoBehaviour
{
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

    [Header("Transition Elements")]
    [SerializeField] Animator animator;
    [SerializeField] CanvasGroup fadeCanvasGroup;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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

    private void ShowImage(Day day)
    {
        switch (day)
        {
            case Day.Monday: monday.enabled = true; break;
            case Day.Tuesday: tuesday.enabled = true; break;
            case Day.Wednesday: wednesday.enabled = true; break;
            case Day.Thursday: thursday.enabled = true; break;
            case Day.Friday: friday.enabled = true; break;
            case Day.Saturday: saturday.enabled = true; break;
            case Day.Sunday: sunday.enabled = true; break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="day"></param>
    /// <param name="onMidTransition"></param>
    /// <returns></returns>
    public IEnumerator PlayDayTransition(Day day, Action onMidTransition = null)
    {
        // Step 1: Fade to Black
        yield return StartCoroutine(FadeToBlack(1));

        // Step 2: Enable Relevant day image
        HideAllDayImages();
        ShowImage(day);

        // Step 3: Play the transition animation
        if(animator != null)
        {
            animator.Play("Day Change");
        }

        // Step 4: Wait for the animation to complete
        yield return new WaitForSeconds(1.5f); // Adjust this duration to match your animation length

        // Step 5: Call the mid-transition action if provided
        onMidTransition?.Invoke();

        yield return new WaitForSeconds(1.5f); // wait time before fading back in

        // Step 6: Fade back in
        yield return StartCoroutine(FadeToBlack(0));

    }

    /// <summary>
    /// Coroutine to Fade in and out to black.
    /// </summary>
    /// <param name="targetAlpha">0 to fade out, 1 to fade in</param>
    /// <returns></returns>
    public IEnumerator FadeToBlack(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = GetComponent<CanvasGroup>();
        }

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;
        float duration = 1f; // Duration of the fade

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha; // Ensure we set the final alpha
    }

    //public IEnumerator DisplayDayOfWeek(Day dayToDisplay)
    //{
    //    HideAllDayImages();
    //    switch (dayToDisplay)
    //    {
    //        case Day.Monday:
    //            monday.enabled = true;
    //            break;
    //        case Day.Tuesday:
    //            tuesday.enabled = true;
    //            break;
    //        case Day.Wednesday:
    //            wednesday.enabled = true;
    //            break;
    //        case Day.Thursday:
    //            thursday.enabled = true;
    //            break;
    //        case Day.Friday:
    //            friday.enabled = true;
    //            break;
    //        case Day.Saturday:
    //            saturday.enabled = true;
    //            break;
    //        case Day.Sunday:
    //            sunday.enabled = true;
    //            break;
    //    }
    //    animator.Play("Day Change");
    //    yield return new WaitForSeconds(3);
    //}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimatorController
{
    [SerializeField] private Animator m_animator = null;
    private RuntimeAnimatorController m_animatorController;

    // Constructor
    public AnimatorController(Animator animator)
    {
        m_animator = animator;
        m_animatorController = m_animator.runtimeAnimatorController;
    }

    public AnimationClip GetAnimation(string animationName)
    {
        AnimationClip[] animationClips = m_animatorController.animationClips;
        AnimationClip foundAnimationClip = null;
        bool animationFound = false;
        int animationIndex = 0;

        while (animationIndex < animationClips.Length && !animationFound)
        {
            if (animationClips[animationIndex].name == animationName)
            {
                foundAnimationClip = animationClips[animationIndex];
                animationFound = true;
            }
            ++animationIndex;
        }

        return foundAnimationClip;
    }

    public float GetAnimationLength(string animationName)
    {
        return GetAnimation(animationName).length;
    }

    public void SetBool(string paramName, bool value)
    {
        if (ParameterExists(paramName))
            m_animator.SetBool(paramName, value);
    }

	public void SetFloat(string paramName, float value)
	{
		if (ParameterExists(paramName))
			m_animator.SetFloat(paramName, value);
	}

	public void SetInt(string paramName, int value)
	{
		if (ParameterExists(paramName))
			m_animator.SetInteger(paramName, value);
	}

    // true value sets trigger, false value resets trigger
	public void ToggleTrigger(string paramName, bool value)
	{
		if (ParameterExists(paramName))
		{
			if (value) m_animator.SetTrigger(paramName);
			else m_animator.ResetTrigger(paramName);
		}
	}

	/// <summary>
	/// Checks if a given parameter exists within the Animator Component
	/// </summary>
	/// <param name="paramName">Name of parameter in Animator</param>
	/// <returns>If the value exists</returns>
    private bool ParameterExists(string paramName)
    {
        bool paramExists = false;

        foreach(AnimatorControllerParameter param in m_animator.parameters)
        {
            if (param.name == paramName)
                paramExists = true;
        }

        return paramExists;
    }
}

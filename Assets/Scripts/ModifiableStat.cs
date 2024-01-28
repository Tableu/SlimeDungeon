using UnityEngine;

namespace Systems.Modifiers
{
    /// <summary>
    /// A class that provides a uniform interface for modifying statistics.
    /// </summary>
    /// <remarks>
    ///  The stat value is constrained to be a non-negative.
    ///  Implicitly casts to a floating point number repressing the current stat value.
    /// </remarks>
    public class ModifiableStat
    {
        public ModifiableStat(float baseValue)
        {
            BaseValue = baseValue;
        }

        /// <summary>
        /// The base value of this statistic with no modifiers applied
        /// </summary>
        public float BaseValue { get; private set; }

        /// <summary>
        /// An additive modifer to the base stat (applied before additive modifiers)
        /// </summary>
        /// <remarks>
        /// A value of 0 indicates no scaling.
        /// </remarks>
        public float BaseModifier { get; internal set; }

        /// <summary>
        /// A percent multiplicative modifer used to scale the base stat. Intended to be used for difficulty scaling.
        /// </summary>
        /// <remarks>
        /// A value of 0 indicates no scaling.
        /// </remarks>
        public float ScalingModifer { get; internal set; }

        /// <summary>
        /// A percent multiplicative modifer used to scale the base stat.
        /// </summary>
        /// <remarks>
        /// A value of 0 indicates no scaling.
        /// </remarks>
        public float MultiplicativeModifer { get; internal set; }

        /// <summary>
        /// The current value of this stat with all modifiers applied.
        /// </summary>
        /// <remarks>
        /// Constraints the current value to be non-negative to prevent potential bugs like dealing negative damage.
        /// </remarks>
        public float CurrentValue =>
            Mathf.Max((BaseValue + BaseModifier) * (1 + ScalingModifer) * (1 + MultiplicativeModifer), 0);

        /// <summary>
        /// Used to set the base value of a stat after construction.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBaseValue(float value)
        {
            BaseValue = value;
        }

        /// <summary>
        /// Implicitly convert to a float for convenience
        /// </summary>
        /// <param name="stat">The modifiable stat to convert</param>
        /// <returns>The CurrentValue of stat</returns>
        public static implicit operator float(ModifiableStat stat) => stat.CurrentValue;
    }
}

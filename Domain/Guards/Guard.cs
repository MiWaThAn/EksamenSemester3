using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Guards
{
    /// <summary>
    /// Guard til at samle vores exception tjek logik på et sted så de mest grundlæggende tjek for null og tomme strenge kan genbruges på tværs af hele domænet uden at skulle skrive det samme kode igen og igen.
    /// Det gør også at vi kan sikre at vores exception beskeder er konsistente og informative, og at vi kan håndtere disse exceptions på en struktureret måde i vores applikation.
    /// Samt kan vi ændre vores tjek logik på et sted hvis vi skulle få brug for at tilføje flere tjek eller ændre på vores exception beskeder, uden at skulle ændre på alle de steder i koden hvor vi bruger disse tjek.
    /// </summary>
    public static class Guard
    {
        public static void AgainstNull(object input, string parameterName)
        {
            if (input == null)
                throw new ArgumentNullException(parameterName, $"{parameterName} må ikke være null.");
        }
        public static void AgainstNullOrEmpty(string input, string parameterName)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException($"{parameterName} må ikke være null eller tom.", parameterName);
        }
        public static void AgainstEmptyGuid(Guid input, string parameterName)
        {
            if (input == Guid.Empty)
                throw new ArgumentException($"{parameterName} må ikke være en tom Guid.", parameterName);
        }
        public static void AgainstInvalidLength(string input, int expectedLength, string parameterName)
        {
            if (input?.Length != expectedLength)
                throw new ArgumentException($"{parameterName} skal være præcis {expectedLength} tegn.", parameterName);
        }
        public static void AgainstInvalidTimeRange(DateTime Start, DateTime End)
        {
            if (Start >= End) throw new ArgumentException("Start dato må være før slutdato.");
        }
        public static void AgainstNegativeOrZero(decimal input, string parameterName)
        {
            if (input <= 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} må ikke være negativ eller nul.");
        }
    }
}

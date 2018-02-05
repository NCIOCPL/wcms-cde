using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrialsAPI.Test
{
    public class StudySiteComparer: IEqualityComparer<ClinicalTrial.StudySite>
    {

        private GeoLocationComparer _locationComparer = new GeoLocationComparer();

        #region IEqualityComparer<StudySite> Members

        public bool Equals(ClinicalTrial.StudySite x, ClinicalTrial.StudySite y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            //This should compare every single property.
            bool isEqual = 
                x.AddressLine1 == y.AddressLine1 &&
                x.AddressLine2 == y.AddressLine2 &&
                x.City == y.City &&
                x.ContactEmail == y.ContactEmail &&
                x.ContactName == y.ContactName &&
                x.ContactPhone == y.ContactPhone &&
                _locationComparer.Equals(x.Coordinates,y.Coordinates) &&
                x.Country == y.Country &&
                x.Family == y.Family &&
                x.LocalSiteIdentifier == y.LocalSiteIdentifier &&
                x.Name == y.Name &&
                x.OrgEmail == y.OrgEmail &&
                x.OrgFax == y.OrgFax &&
                x.OrgPhone == y.OrgPhone &&
                x.OrgToFamilyRelationship == y.OrgToFamilyRelationship &&
                x.OrgTTY == y.OrgTTY &&
                x.PostalCode == y.PostalCode &&
                x.IsVA == y.IsVA &&
                x.RecruitmentStatus == y.RecruitmentStatus &&
                x.StateOrProvince == y.StateOrProvince &&
                x.StateOrProvinceAbbreviation == y.StateOrProvinceAbbreviation;                

            return isEqual;

        }

        public int GetHashCode(ClinicalTrial.StudySite obj)
        {
            int hash = 0;
            if (obj.AddressLine1 != null)
                hash ^= obj.AddressLine1.GetHashCode();

            if (obj.AddressLine2 != null)
                hash ^= obj.AddressLine2.GetHashCode();

            if (obj.City != null)
                hash ^= obj.City.GetHashCode();

            if (obj.ContactEmail != null)
                hash ^= obj.ContactEmail.GetHashCode();

            if (obj.ContactName != null)
                hash ^= obj.ContactName.GetHashCode();

            if (obj.ContactPhone != null)
                hash ^= obj.ContactPhone.GetHashCode();

            if (obj.Coordinates != null)
                hash ^= _locationComparer.GetHashCode(obj.Coordinates);

            if (obj.Country != null)
                hash ^= obj.Country.GetHashCode();

            if (obj.Family != null)
                hash ^= obj.Family.GetHashCode();

            if (obj.LocalSiteIdentifier != null)
                hash ^= obj.LocalSiteIdentifier.GetHashCode();

            if (obj.IsVA != null)
                hash ^= obj.IsVA.GetHashCode();

            if (obj.Family != null)
                hash ^= obj.Family.GetHashCode();

            if (obj.Name != null)
                hash ^= obj.Name.GetHashCode();

            if (obj.OrgEmail != null)
                hash ^= obj.OrgEmail.GetHashCode();

            if (obj.OrgFax != null)
                hash ^= obj.OrgFax.GetHashCode();

            if (obj.OrgPhone != null)
                hash ^= obj.OrgPhone.GetHashCode();

            if (obj.OrgToFamilyRelationship != null)
                hash ^= obj.OrgToFamilyRelationship.GetHashCode();

            if (obj.OrgTTY != null)
                hash ^= obj.OrgTTY.GetHashCode();

            if (obj.PostalCode != null)
                hash ^= obj.PostalCode.GetHashCode();

            if (obj.RecruitmentStatus != null)
                hash ^= obj.RecruitmentStatus.GetHashCode();

            if (obj.StateOrProvince != null)
                hash ^= obj.StateOrProvince.GetHashCode();

            if (obj.StateOrProvinceAbbreviation != null)
                hash ^= obj.StateOrProvinceAbbreviation.GetHashCode();

            return hash;
        }

        #endregion
    }
}

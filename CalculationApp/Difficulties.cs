//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CalculationApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Difficulties
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Difficulties()
        {
            this.Tasks = new HashSet<Tasks>();
        }
    
        public int IdDifficulties { get; set; }
        public string Name { get; set; }
        public int IdCategory { get; set; }
        public int PercentageOfCost { get; set; }
    
        public virtual CategoriesOfEmployees CategoriesOfEmployees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}

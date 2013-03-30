using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FoodVariable.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : FoodVariable.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "In Pakistan and India every woman knows that bride is incomplete without applying beautiful and stunning mehndi designs on her hands, feet and arms. These days, applying mehndi becomes popular fashion. Every year, numerous mehndi designs are coming for women and young girls. In this post, we are presenting latest and exclusive mehndi designs 2013 for women. Women and girls can apply these mehndi designs on their hands, arms and feet. All mehndi designs 2013 are simply stunning and magnificent. These mehndi designs 2013 include different types of designs like floral designs, peacock designs and many more. If we talk about these mehndi designs then some mehndi designs are extremely beautiful but difficult. So women can apply them with the help of professional mehndi artist. On the other hand, some of them are simple then even girls can easily apply them without taking any help.");

            var group1 = new SampleDataGroup("Group-1",
                 "Home Remedies",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs. In this style we can use different styles of mehndi like Black mehndi is used as outline, fillings with the normal henna mehndi. We can also include sparkles as a final coating to make the henna design more attractive.");

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item1",
                 "Removing Your Acne Scars Naturally at Home",
                 "Removing Your Acne Scars Naturally at Home",
                 "Assets/HubPage/HubpageImage2.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "When your pores get clogged with oil, dirt, and old skin cells, they can become inflamed and swell up into what we recognize as the common garden variety zit. Fortunately, most of us don't (or didn't) have acne severely enough to leave huge scars, though if you had a big one that you popped, chances are your skin isn't as smooth as it once was on that spot. The ac-tor Tommy Lee Jones' skin isn't rough because he's so manly, it's acne scars. So now that you've identified that you have acne scars, what the heck can you do about it? Common treatments include surgery, laser resurfacing, or even dermabrasion, in which the doctor basically grinds the rest of your skin off, then waits for it to heal smoothly. Since they are all medical procedures, all of these methods require a doctor and/or expensive equip-ment. If you aren't interested in cutting the skin on your face more, or some guy shooting lasers at you, or basically what amounts to a doctor using an angle grinder on your face, you'll be in the market for some home remedies for acne scar removal. \n\nA popular method for scar removal/reduction is to rub ice on your scars to shrink the pores and reduce their appearance. This is a short lived method, since once the pores realize they aren't actually frozen, they go back to what they used to be. This is the cheapest, least in-trusive of the home remedies for acne scar removal. \n\nCucumber juice essentially does the same thing. If you can conjure up images of women basking in warm glow of a facial mud mask with cucumber slices over her eyes, you'll get the idea. The juice has something in it that shrinks pores and reduces the appearance of scars. Another, more scientific method is to use a paste made of sandalwood, rose water, and milk. OK, it's scientific in principal, but pretty hippy friendly overall, but the idea is that the chemicals in the sandalwood help condition and soften the scar tissue, shrink your pores, and actually work to bring the scar up close to the level of the skin around it. Pretty cool, huh?\n\nJust remember, an acne scar is just that: it's a scar. It is scar tissue that does not share the same characteristics as regular skin, so scarring is a permanent thing. Home remedies for acne scar removal are not permanent fixes, and often merely reduce the appearance of scars. ",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Home Remedies For Acne Blemishes",
                 "Home Remedies For Acne Blemishes",
                 "Assets/HubPage/HubpageImage3.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Acne is the bane of many an individual's existence! Unfortunately, there are no guarantee's with any course of treatment. It can just end up making your acne worse if you aren't careful, due to the harsh ingredients found within many medications and acne cleansers. Home remedies for acne blemishes are usually a great course of action to take however. This is simply because you can usually avoid all of the nasty side effects with them that are associated with prescription and over the counter medications.So, what are some good home remedies for acne blemishes? Here are a few that have been found to be effective for some. Please bear in mind though, that not all of them work for everyone, so don't be disappointed if a particular remedy doesn't work for you.\n\nBaking soda is one of the really good home remedies for acne blemishes, simply because it can be used both internally and externally. Baking soda has been shown to kill the yeast over growth called Candida that is thought to also trigger flare ups of acne, among a whole host of other health issues.\n\nYou can create a topical face mask by mixing baking soda and water and spreading the paste on your skin. Allow it to dry and then wash your face with warm water. It kills any bacteria lying around on your skin and will leave your face feeling refreshed and clean. In addition, you can also take baking soda orally to kill an overgrowth of yeast inside your body and to make it more alkaline. Too much acid can cause acne flare ups as well, so making the body more alkaline can greatly reduce that inflammation.  Simply mix a teaspoon or two in some water and drink it twice a day, or try getting some empty veggie capsules and putting the baking soda inside. You can then swallow a couple capsules twice a day and never be bothered with the taste of baking soda. \n\nBe aware though that if yeast is high in your body, the first couple of days of taking this orally may cause some adverse effects like headache or tummy troubles. This is simply because it is killing off the yeast which is causing the toxicity levels in your body to rise. Drink lots of water and this will help flush all that nastiness out. This is one of many simple home remedies for acne blemishes that you can try - if it doesn't work for you just try another. Don't get discouraged, because eventually you will hit upon the one that makes a difference for you, and your acne will then be but a distant memory!",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item3",
                 "Fix Your Acne at Home",
                 "Fix Your Acne at Home",
                 "Assets/HubPage/HubpageImage4.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Acne is a blight upon the teenage race. Teens and adults alike wage an almost constant war-fare on the condition, which causes blackheads, puss-filled inflammation on the skin of the face, back, scalp, and just about anywhere else that is likely to get dirty and/or oily. Regular skin care is essential to treating and preventing acne, but since the most common root cause is not necessarily oil or dirt, but genetics, acne can rear its head any time, any-where, for any person. Despite Hollywood's best efforts to hide them, even movie stars with so called perfect skin get pimples. Luckily for the rest of us, we don't have to spend a for-tune on acne treatments. With some simple home remedies for acne, you can keep your face looking smooth, clear, and radiant for years to come.\n\nApplying papaya juice in its raw form will help shrink and eliminate pimples. When you include the pulp of the fruit, the skin, and the seeds, you get the full reduction benefits of the papa-ya fruit. Apply the juice evenly and leave it on for 20-30 minutes, then wash it off with warm water. This will help to condition and exfoliate your skin to smooth it out and clear up un-sightly blemishes. \n\nIf you can get your hands on it, sandalwood powder is a powerful remedy for acne. Mix it up into a paste with either lime juice or high fat content milk. Apply the paste and let it sit for a while, then wash it off. The milk will moisturize and the sandalwood will help to soak up the extra oils. Since sandalwood powder is pulverized sawdust that has already had the essential oils removed, it soaks up oil very well. Another good remedy for acne, although it is a bit slow working, is drinking water. Your body needs water, and if it doesn't get enough, it diverts water from less important areas to more important ones. While your skin is certainly important, your heart, lungs, and stomach are a bit higher priority, so in the absence of enough water, it takes it from your skin until you re-plenish it. Avoid this by drinking plenty of clean water. \n\nMedicinal remedies are fine, and they work, but at what cost? Some of them are even petro-leum based, or use harsh solvents to dry out your skin. Only the nice, high priced ones are actually good for your skin, but the cost can be prohibitive for most people. Home remedies for acne are the way to go if you want to be environmentally responsible and naturally healthy",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Get Rid of Your Acne Naturally",
                 "Get Rid of Your Acne Naturally",
                 "Assets/HubPage/HubpageImage5.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Curing acne at home sounds like a daunting task. It really isn't, though! There are many home remedies for curing acne, and a vast majority of them are easy to do, easy to under-stand, and quite possibly cost free. Over the counter medicinal solutions may work, but many of them contain harsh chemicals. Prescription medications have the same chemicals, but they're just stronger than the over the counter counterparts. The other downside to pharmaceuticals is the cost. Drugs cost more than home remedies for curing acne, and in some cases, they aren't s effective. One popular home acne remedy is to rub garlic on your pimples. This will help reduce the ap-pearance of acne and keep it away. Simply rub a freshly cut slice of garlic over the offend-ing blemish and allow the juices and oils to seep into it. Wash it off a short while later, say about a half an hour, and your pimples will be noticeably smaller. This remedy, of course if only for you if you don't mind smelling like garlic. \n\nIf you take some peanut oil an mix it with an equal amount of lime juice and you have a vir-tual pimple assassination mix. The peanut oil conditions your skin while the lime juice is acidic enough to gently smooth your skin while peeling off a very thin layer of old skin cells. As with the garlic, simply rub the mixture on the affected area and wash it off after about a half an hour. You should see results almost right away. Avoid this mixture if you are allergic or sensitive to peanuts or any other kind of ground or tree nut. \n\nA slightly more involved recipe involves pomegranate skin and lime juice. First you have to roast the pomegranate skin until it is sufficiently dried out, then you crush using either a small high powered blender or a mortar an pestle. Once you have roasted pomegranate skin power, you can then mix it up with lime juice to form a paste that works wonders on acne and all of is related skin problems. While you will probably be able to find the drug store acne remedies more available or easier to find, it pays in the long run to use the more natural methods. Environmental concerns in recent years have increased the popularity of health food stores and other stores that advo-cate natural and healthy living. The bottom line is that while chemical treatments work, home remedies for curing acne can be just as effective, cheaper, and in some cases interestingly aromatic. Try one or two of them, you'll like it.",
                 69,
                 70,
                 group1));

            group1.Items.Add(new SampleDataItem("Landscape-Group-1-Item5",
                 "Home Remedies To Help Cure Acne",
                 "Home Remedies To Help Cure Acne",
                 "Assets/HubPage/HubpageImage6.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Acne can be a horrible drain on your self esteem. Unfortunately, there are many people suffering from it, and there doesn't seem to be a whole lot of success in finding good treatments. Home remedies to help cure acne may be the best course of action for you to follow if you are worried about the side effects of prescription medications and over the counter treatments. There are literally hundreds of home remedies to help cure acne available. It's just a matter of finding the one that works for you. Just be warned that not every method works for every person because everyone's body composition is different.Garlic is one of the home remedies to help cure acne that is used with some success. Garlic has natural antibiotic and antiseptic properties that is great for skin that is inflamed and teaming with infectious bacteria.\n\nYou can use it in several different ways, including eating it and taking garlic supplements. However, as one of the home remedies to help cure acne, it is best to apply it directly to the skin. You can do this by rubbing fresh garlic on and around the pimpled area.The same can be said for mint as well - it has many healing properties. You can use it by applying fresh mint juice all around the infected areas. Vinegar also has great antiseptic properties as well that you can apply to the face as yet one more of several home remedies to help cure acne.\n\nOne thing you want to avoid however is doing things to further irritate sensitive acne prone skin. Things like scrubbing your face too hard can cause acne to worsen and flare up even more. Also using harsh cleansers that dry out the skin can cause it to flake and increase inflammation. \n\nYou also want to make sure to wear sunscreen each day, as sunburned skin can cause acne to get worse as well. The trick is to keep inflammation down as much as possible, as inflammation is what cause the angry redness that is common to acne sufferers. Finally, just remember that there are no home remedies to help cure acne that are perfect, and not every single one is safe despite being called natural. If you have sensitive skin make sure you do your research before trying it, as it'd be a shame to make things worse instead of better!",
                 69,
                 35,
                 group1));

            

            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "Natural Remedies",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                "Natural Acne Remedies Available at Your Supermarket",
                "Natural Acne Remedies Available at Your Supermarket",
                "Assets/HubPage/HubpageImage7.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "If you struggle with embarrassing acne, you may be tempted to spend money on the hottest new product advertised on television or in your drugstore. Unfortunately, many of these products are overpriced, ineffective, or simply don't work for your skin type. Instead of in-vesting a fortune trying to heal your acne, consider using natural acne remedies. Many in-gredients for natural acne remedies are available at your supermarket. In fact, you may al-ready have them in the kitchen. You may not know it, but toothpaste is an excellent treatment for acne. We've all had one of those pimples that popped up seemingly overnight. These blemishes can be especially embar-rassing the morning before the big event, speech, or presentation. It's possible, however, to treat sudden blemishes quickly and effectively using toothpaste. For this remedy, you will need a paste toothpaste, not a gel. \n\nSimply squeeze out a small amount of toothpaste and apply it to your pimples. Leave it sit on your skin for 15 to 20 minutes, or overnight for best results. Rinse it off with mild soap and water, and you will notice your pimple is noticeably smaller and the redness has been re-duced. If you have extremely sensitive skin, use caution with this method, as if you leave the toothpaste on for too long your skin may feel burned or irritated.\n\nAnother natural acne remedy is citric acid. Although this sounds fancy, it's actually an ingre-dient found in citrus fruit. This means if you have lemons, limes, oranges, or grapefruits in your kitchen, you have a remedy for your acne. You can either squeeze the fruit, and apply the juice directly to your pimple, or you can grind the peel into a paste and apply to your skin as a poultice. Either way, the acid acts to smooth skin, reduce redness, and fight bacteria.\n\nSalt and vinegar mixed together to form a paste is another excellent and inexpensive acne remedy. Simply pour some salt in a dish, and add enough vinegar to form a paste. Apply this paste to your skin and leave on for five to 10 minutes. Then wash your face with mild soap and lukewarm water. You should notice a visible difference.\n\nThere you have it - three easy, inexpensive, and natural remedies for acne. These natural acne remedies can be mixed up in your own kitchen using common ingredients you can pur-chase at your local super market. Never spend money on a costly acne remedy again. In-stead, treat your blemishes in the privacy of your own home",
                69,
                70,
                group2));

            group2.Items.Add(new SampleDataItem("Landscape-Group-2-Item2",
                "Fight Blemishes Now",
                "Fight Blemishes Now",
                "Assets/HubPage/HubpageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Natural remedies for acne are an easy way to treat your blemishes at home. Whether you struggle with the occasional pimple or have a full blown case of recurring acne, there are natural remedies available to help you treat your condition. With a trip to the grocery or health food store, you can get your acne under control quickly and easily using these simple acne remedies. Did you know that you could use cucumber as a facial mask to help prevent acne? By leaving them out scorn for 30 minutes per day, you can prevent acne breakouts naturally. this sim-ple natural remedy is inexpensive, and will make your skin look great.\n\nAnother easy natural remedy for acne is garlic. If you would like to keep vampires away and cure your acne at the same time, this is the home remedy for you. Simply mash or mince fresh garlic, and apply to the blemished area of your face. Your acne will disappear after several applications of garlic. If you decide to use this method, avoid using purchased minced garlic packed in oil. Instead, choose fresh garlic from the produce section, and mash it your-self. Cow's milk is also useful as an acne remedy when combined with nutmeg. Although this con-coction may sound more like eggnog than medication, it's actually quite useful. Blend pow-dered nutmeg with enough milk to make a paste. Apply this paste to your skin daily in areas where acne occurs. The mixture will act to shrink pimples and heal your skin. \n\nTomatoes are another natural acne fighter. Choose a few ripe tomatoes from your local su-permarket and mash them to a pulp. Apply this pulp to your face, or anywhere else that you struggle with acne. Leave the paste on for up to one hour, then rinse away with warm wa-ter. The acid in the tomato acts to smooth your skin and fight blemishes, making this fruit a powerful acne fighting machine. Potato is another vegetable that can be used to treat acne. Wash and grate a potato, then use the shavings. Potato is useful for treating wrinkles, pimples, blackheads, whiteheads, and other skin conditions. After you've applied the potato poultice, be sure to wash your face with mild soap and water.\n\nJust because you struggle with acne doesn't mean the have to spend a fortune to get it cor-rected. In fact, there may be no need for costly over-the-counter products or dermatologist treatments. Instead, you can put simple ingredients from your kitchen to work for you. Take advantage of the natural remedies for acne lurking in your kitchen cabinets and refrigerator today.",
                69,
                35,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item3",
                "Overnight Home Remedies For Acne",
                "Overnight Home Remedies For Acne",
                "Assets/HubPage/HubpageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Unfortunately, there are no overnight home remedies for acne. Acne takes time to heal, and not all treatments work for everyone. Even prescription medications take time, so if you are looking for a magic bullet, you are sure to be disappointed.However, despite the fact that there are no overnight home remedies for acne, there are some things you can do to help combat it, and hopefully banish it for good. A huge thing to keep in mind is cutting down on your vegetable oil consumption.Vegetable oil has been shown to trigger a flare up of acne inflammation in as little as 30 minutes of consumption. Unfortunately it's hard to find food that DOESN'T contain vegetable oil, which could be the reason acne is so prevalent. \n\nJust read food labels carefully, and up your intake of natural whole foods, and that should cut down on your vegetable consumption significantly. Overnight home remedies for acne are a myth, but simple diet tweaks can work wonders. Another thing that you can avoid that may help with your acne is dairy products. Unfortunately there are many folks who are allergic to dairy and never realize it because their reactions aren't necessarily 'typical'. But dairy really isn't meant for humans, and our bodies have a tough time digesting it. Try almond milk instead as a great low fat alternative.Another biggie is refined sugars. The bulk of food on the market today is full of refined sugars and carbs, and does our bodies absolutely no favors whatsoever. Again, if you fill up on mostly whole foods like fresh fruits and veggies, and look for complex carbs like whole grain breads, this will reduce your refined sugars considerably. \n\nAlso remember to drink lots of water. It may not be touted as one of the super hero overnight home remedies for acne, but you'd be amazed at how much increasing your water intake improves the quality and elasticity of your skin. Water flushes out toxins, and keeps your body hydrated so that all systems work at their optimum levels. That includes your skin. So the next time you are hunting up overnight home remedies for acne, give it up and look to making some real long term changes instead. You will see much more success along that road than any other.",
                41,
                41,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item4",
                "What Are Natural Remedies for Acne?",
                "What Are Natural Remedies for Acne?",
                "Assets/HubPage/HubpageImage09.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "There has been a boom recently in interest about all things natural. Whether someone is looking for a replacement for a synthesized product they have grown accustom to, or they are simply looking for new things that they can do without relying on the chemical industry. Will explore some natural acne treatments here. What are natural remedies for acne?, you might ask. Well, it's just what it sounds like! Anything that comes from nature that can be used to reduce the swelling of acne, clean the pores, or otherwise reduce the appearance of acne related blemishes. Here are a few examples. Citric acid is an excellent treatment for pimples of all sizes and types. A direct cause of pim-ples is the buildup of dirt and oil in your pores, which then become clogged, inflamed, and infected. The citric acid found in all citrus fruits is absolutely perfect for breaking that oily buildup down, taking away the home for dirt and dead skin cells and allowing these irritants to be washed away harmlessly. Using straight orange juice isn't enough, though; you have to find a stronger fruit (more sour) such as lemons, limes, or even grapefruits. The peel actually has the highest concen-tration of acid, so take some of the dark colored rind of the fruit, dry it and make it into a paste with water or even lime juice. The acid will cleanse your skin and leave you with a fresh clean feeling, not to mention a nice smell.\n\nAnother simple thing you can do to fight the symptoms of acne is to mash up some sesame seeds and make it into a paste using either lime juice or milk as a base. Apply the paste, then wash it off later, and you will see noticeable results in a relatively short time frame. Rounding out some favorite natural remedies for acne is nutmeg and milk. This gives you roughly the same effect as acid, seeds, or anything else, except that the milk helps you to condition your skin, leaving a nice healthy glow. Never forget that your skin is the biggest organ of the body and quite possible one of the most important (next to heart and lungs, of course). Take care of it- wash it often and thor-oughly, but not too vigorously, and it will stay clear for the most part. If you are genetically disposed to bad acne, the best bet for you is probably treatment of the symptoms using stronger medicine, but before you take that leap, ask your doctor what are natural remedies for acne.",
                41,
                41,
                group2));

            
            this.AllGroups.Add(group2);


            

            var group3 = new SampleDataGroup("Group-3",
               "Herbal Remedies",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("Big-Group-3-Item1",
                "Herbal Remedy #1",
                "Herbal Remedy #1",
                "Assets/HubPage/HubpageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "An easy way to treat acne is by applying peanut oil and lime juice to your pimples. Mix equal parts lemon or lime juice and peanut oil to form a solution. Put this liquid on the individual blemishes each night and leave it on for 15 minutes while you tidy up, take a bath, etc. Then rinse the solution off with warm water. This solution will help prevent pimples and black heads. Avoid using this solution if you have a peanut or legume allergy, however, as it can cause severe reaction. ",
                69,
                70,
                group3));

            group3.Items.Add(new SampleDataItem("Landscape-Group-3-Item2",
                "Herbal Remedy #2",
                "Herbal Remedy #2",
                "Assets/HubPage/HubpageImage11.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Another easy acne herbal remedy is rose water. Lime juice mixed with rose water will help to shrink your pores and cleanse your skin. Over time, application of this solution will lead to clear, conditioned skin. Rose water is available at many health or natural food stores, and is quite inexpensive. Store the solution in a bottle in your refrigerator for a refreshing face wash - it will keep for many weeks. ",
                69,
                35,
                group3));

            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item3",
                "Herbal Remedy #3",
                "Herbal Remedy #3",
                "Assets/HubPage/HubpageImage12.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Rose water can also be mixed with sandalwood or tea tree essential oils. Apply the solution to your blemishes each night, leave on for ten minutes, then rinse with cool water. The essen-tial oils help fight bacteria on your skin, while the rose water works to cleanse and purify your skin. The result - clean, clear, blemish free skin. This solution can also be stored in the fridge for a refreshing face wash.Herbal remedies for acne are quick to mix up, store for long periods of time, and can drasti-cally improve your skin with regular use. If you're interested in clearing up your skin, don't bother investing in costly drug store remedies. A trip to your local health food store can pro-vide you with excellent results inexpensively. Best of all, you'll know that the products that you're putting on your skin are safe and natural.",
                41,
                41,
                group3));
            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item4",
               "Herbal Remedy #4",
               "Herbal Remedy #4",
               "Assets/HubPage/HubpageImage13.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Acne is a troublesome and unsightly skin condition often seen in teenagers, but can also be found in adults of any age, and even some young children. The main cause of acne is genet-ics, but the outward symptoms (pimples and black heads) are caused by excessively oily skin, dirt, and even dehydration. Remember that the skin is the largest organ in the body, and is one of the most obvious indicators of overall health. Clear skin is a sign of a healthy body, and acne riddled skin can be a sign that there is some sort of imbalance in the body. More often than not, though, it is simply a matter of a less than perfect skin care regimen. Steps can be taken to naturally control and cure acne. Here are some herbal remedies for acne.One of the easiest of the herbal remedies for acne is using groundnut oil (the most common is peanut oil) and mixing it with and equal amount of lime juice. Mix well and apply the solu-tion to your face every night for about 15 or 20 minutes. After that short while, rinse it off with warm water. Daily application can help prevent the formation of black heads, which eventually turn into full blown pimples when they get infected. It may be obvious, but don't use this method if you are allergic to peanuts. \n\nIf you happen to be allergic to peanuts, try mixing the lime juice with rose water in equal portions. The rose water doesn't moisturize as well as the peanut oil, but its unique proper-ties will shrink your pores and the citrus juice will help cleanse them, leaving your skin clean and clearer. You can find rose water at most health food or natural living stores.\n\nAnother use of rose water is to mix it with sandalwood. Sandalwood is a popular wood for use in fragrances and botanicals, and it's essential oil has antibacterial properties. The rose water helps condition your skin, while the sandalwood helps to fight the bacteria that can cause pimples to form. ",
               41,
               41,
               group3));

            this.AllGroups.Add(group3);


         



            var group4 = new SampleDataGroup("Group-4",
               "Useful Tricks to Cure Ance",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item1",
               "Fast Remedies for Acne",
               "Fast Remedies for Acne",
               "Assets/HubPage/HubpageImage14.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "We've all had one of those mornings. You wake up, ready to prepare for your important presentation, interview, or date, only to see a large pimple glaring back at you in the mirror. Instead of canceling your special day, consider using one of these fast remedies for acne. You'll be pleasantly surprised at how quickly and effectively they work. One easy remedy for acne that works quickly can be found in your bathroom. Simply find a tube of paste toothpaste (not gel), squeeze out a bit, and apply it to your blemish. Leave it on for 5 or 10 minutes for a noticeable reduction in redness and swelling. Toothpaste is a quick and inexpensive way to fight acne fast. \n\nAnother fast remedy for acne is available over the counter - salicylic acid. This ingredient, found in many popular acne medications, acts to quickly treat redness, swelling, and infection caused by acne. Look for this ingredient in a variety of acne lotions, creams, and products - it's even available in coverup - allowing you to treat your pimple and conceal it at the same time. Citric acid also works to fight acne. This ingredient is found in citrus fruit, such as lemons, limes, and oranges. The acid helps to smooth skin, reduce redness, and fight bacteria. You can either squeeze some of the juice onto your finger and apply to your pimple, or you can blend the peels in the blender with a bit of water to form a paste and apply that. Either method works as a fast remedy for acne. \n\nAnother good acne fighter is salt and vinegar. The salt acts to dry out the inflamed and irri-tated skin of the pimple while the acid in the vinegar breaks down the harmful oils and cleans out the dirt. The vinegar also acts on the skin in general to smooth and exfoliate it. Turn the salt and vinegar into a paste, apply it and wash it off after about 20 minutes. All of these fast remedies for acne are equally effective, though some are cheaper than oth-ers. The pharmaceutical solutions to problem acne may work better than some of the home-spun remedies, but unless you're in that unlucky group of people with severe acne, you will most likely do just as well with citric acid, salt, or vinegar. ",
               41,
               41,
               group4));

            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item2",
                "Quick Acne Remedies",
                "Quick Acne Remedies",
                "Assets/HubPage/HubpageImage15.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Acne remedies are plentiful and varied, but how many of them can you make at home? As it turns out, lots of them! Some natural living stores stock supplies to make your own creams or distill your own oils. That stuff is great, but some people are trying to strike a balance be-tween living naturally and living in today's fast paced, ready made world. Spending hours just mixing the ingredients or getting the recipe right can be truly draining on a person's time and patience, especially if the remedy isn't the right one for you. Using quick homemade acne remedies is a good way to try them to see if they work. You can even try several a day since they are quick to make and usually pretty inexpensive.\n\nTurmeric powder is easy to find in just about any grocery store that carries spices. Mix it with a little mint juice and you can use the paste to reduce the size of large zits and com-pletely eliminate smaller ones. Mint juice is commercially available from numerous health food and natural living stores, but if it isn't, try taking some mint leaves and making a very strong tea from it. Though not as effective as the pure juice of the leaves, you'll get some of the same effects using this very strong tea- the stronger the better.\n\nYou can grind up some sesame seeds and mix them into a paste using water and apply that to any swollen area of the skin, though it works on acne as well. You have to mix it up so it is a very thick consistency, and apply it for a while to make it work. Again, sesame seeds are very easy to find and relatively inexpensive, and water is just about free. One of the quick homemade acne remedies that so many people overlook is toothpaste. You don't need a whole lot of it, you probably already have some in your house, and it works great. The only reason it isn't very popular is that people seem to have some hang up about using a tooth product on their face, as if it will hurt them. With the other remedies you see here, you apply it and leave it on for a half an hour, but with toothpaste, you can apply it before bed and wash it off in the morning!",
                41,
                41,
                group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item3",
               "Holistic Remedies For Acne",
               "Holistic Remedies For Acne",
               "Assets/HubPage/HubpageImage16.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Holistic remedies for acne vary far and wide. Some work, some work slightly, and some are just never going to work for you. It's hard to find the good ones, you just have to dig and keep trying until you hit the jackpot. First, you should understand a bit more about what acne is.Put simply, acne is plugged pores. It can affect just about anywhere on your body, but is most prevalent on the face, neck, back shoulders, arms and buttocks. It's most common in young adults and teenagers, but can occur in every age group.While acne won't kill you, due to it's propensity for scarring, it can leave a person with a very poor self image of themselves and do some major emotional damage. There are several contributing factors, including pregnancy and PMS which trigger hormonal changes, as well as poor diet, stress, and lack of proper hygiene. \n\nA few holistic remedies for acne include things like drinking apple cider vinegar. Be sure to dilute it with water though as the taste is extremely strong, and you may not like it. Apple cider vinegar works by alkalizing the body, which reduces acid and inflammation and kills of any Candida overgrowth.\n\nOther holistic remedies for acne include using tea tree oil. Tea tree oil is a natural astringent used for a variety of skin ailments, including acne. You can also try rinsing your face with calming herb teas like lavender and chamomile.There are tons of other holistic remedies you can try, including green tea extracts, oatmeal, and aloe vera. However, none are as effective as simple lifestyle changes that you can start making today to improve your acne and heal your skin.\n\nHolistic remedies for acne are most successful when they work on the health of your whole body, not just your skin. To that end, you should be trying to eat healthier foods, such as whole grains, and fresh fruits and veggies, as well as getting regular exercise each day.\n\nAvoid excessive use of antibiotics and over the counter medication, as these drugs tend to break down your natural immunity. This is very important when trying out holistic home remedies for acne, because if you natural immune system is low, then it's much more difficult to achieve success.So drink lots of water, eat your veggies, and start making your whole body healthy. That is one of the best holistic remedies for acne that you will ever find.",
               41,
               41,
               group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item4",
               "Three Free or Cheap Acne Remedies",
               "Three Free or Cheap Acne Remedies",
               "Assets/HubPage/HubpageImage17.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Would you believe that treating your acne can be easy, painless, and nearly free? It can! There are literally thousands of acne remedies out there in the world, but so many of them are prohibitively expensive or use chemicals that are not environmentally friendly, that you may not know where to turn. While your choices for cheap or free acne remedies are a little more limited than if you wanted to spend lots of money, there are still plenty options availa-ble that are affordable and effective.One of the cheapest ways you can get rid of unsightly acne is to mash up a ripe tomato and apply the pulp to your face. The riper a tomato is, the more acidic the juice gets. So, it will work better if the tomato is just a bit overripe (but not spoiled, please). The gentle acid and the high water content work together to condition the skin and cleanse the pores of oil and dirt. Leave the pulp on your face for about a half hour, then wash it off with warm water. Don't use soap, as it will undo all of the moisturizing effects of the pulp. \n\nIf the idea of lathering up with tomato seems a bit strange and messy to you, you could try making a paste out of dried and ground up orange peels with water. The paste contains the same strength acid as the tomatoes, but it isn't nearly as watery or messy. Many people also find the scent of citrus to be quite refreshing in the morning. Even in the winter, oranges can be found for less than a dollar, and water is essentially free. While tomatoes and oranges are quite inexpensive, these two methods aren't free acne remedies, but they are awfully cheap.\n\nIf you're looking for absolutely free acne remedies, just look to your tap. Even though you may pay for water through your municipal source, 8 glasses of water per day is essentially free, and may be the easiest and most effective long term method for fighting acne. Pimples are caused by the buildup of oils and dirt- things that would normally be washed away through the pores of the skin naturally. The body needs water for every biological function it does, and when it doesn't have enough, it takes it from lower priority areas to divert it to more important systems. In other words, your body would rather have a pimple than not be able to digest food. Combating acne with water is as simple as drinking enough of it for your body to function. If you drink a little more than you need, that's OK too, since your body will use what it needs and get rid of the rest harmlessly. ",
               41,
               41,
               group4));
            this.AllGroups.Add(group4);



        }
    }
}

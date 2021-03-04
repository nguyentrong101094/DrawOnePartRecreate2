#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("zhkiGl4OxsGCTwWUl8YPFzJ8oJZI8eJllTtakeVNjbAXtUrwflqZ+wOxMhEDPjU6GbV7tcQ+MjIyNjMwYWkgCvAg2AupTrmP36JPoEi3dvOooNacn8bwwrXL+EnPidnjFKLnSWfWTpvRODNJoniXzg1Fp827vQ/2x63FCsxhuv5GILxVYvEmdKazTqQ87cASIbzUmJhOemR6EKfCGQAUIRH9urr/g8RKWdQvyucPjZwAIJFesTI8MwOxMjkxsTIyM68G1W7mEk2vx/HkTxsAFyeRKSfva/JKvYs7XMZVmzkbSopmT8BRhUhjcRUAZq8SyEIgDSi5EicZEMchcwVtM9cNQ1/E5sa44BXLaYaQ9h+WVGPwzlsi0RMh6wVh3l1OHjEwMjMy");
        private static int[] order = new int[] { 5,4,5,11,7,8,10,9,11,9,12,13,12,13,14 };
        private static int key = 51;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif

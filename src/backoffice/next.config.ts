import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "www.elgiganten.se",
        search: "",
      },
    ],
  },
  output: "standalone",
  /* config options here */
  logging: {
    fetches: {
      fullUrl: true,
    },
  },
};

export default nextConfig;

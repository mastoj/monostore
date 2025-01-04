import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  /* config options here */
  logging: {
    fetches: {
      fullUrl: true,
    },
  },
};

export default nextConfig;
